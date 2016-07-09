using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Hale_Core.Config;
using Hale_Core.Contexts;
using Hale_Core.Entities;
using Hale_Core.Entities.Checks;
using Hale_Core.Entities.Nodes;
using NLog;
using Piksel.Nemesis;
using Piksel.Nemesis.Security;
using HaleLib.JsonRpc;
using HaleLib.Modules;
using Hale_Core.Entities.Modules;
using HaleLib.Modules.Checks;
using Newtonsoft.Json.Linq;
using HaleLib.Generalization;
using HaleLib.Utilities;
using System.Configuration;
using Hale_Core.Utils;

namespace Hale_Core.Handlers
{
    class AgentHandler
    {
        private readonly ILogger _log;
        private readonly Hosts _hosts;
        private readonly Checks _checks;
        private readonly ModuleFunctions _functions = new ModuleFunctions();
        private readonly Modules _modules = new Modules();
        private readonly Results _results;

        private NemesisHub _nemesis;
        
        private readonly string _agentKeyStorePath;
        private readonly string _coreKeyFilePath;
        private XMLFileKeyStore _xmlFileKeyStore;

        private readonly AgentSection _agentConfig;
        private readonly Dictionary<Guid, int> _hostGuidsToIds;



        public AgentHandler()
        {
            var agentConfig = ServiceProvider.GetServiceCritical<Configuration>().Agent();

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string keyRepoPath = Path.Combine(appDataPath, "Hale", "Core");

            _agentConfig = agentConfig;
            _hostGuidsToIds = new Dictionary<Guid, int>();
            
            _coreKeyFilePath = Path.Combine(keyRepoPath, "core-keys.xml");
            _agentKeyStorePath = Path.Combine(keyRepoPath, "HostKeys");

            _log = LogManager.GetCurrentClassLogger();

            _hosts = new Hosts(); 
            _checks = new Checks();
            //_checkDetails = new CheckDetails();
            _results = new Results();


            LoadXmlFileKeyStore();
            LoadNemesisClient();
        }

        private void LoadXmlFileKeyStore()
        {

            _xmlFileKeyStore = new XMLFileKeyStore(_coreKeyFilePath, true);

            if (!_xmlFileKeyStore.Available)
                _xmlFileKeyStore.Save();

        }

        private void LoadNemesisClient()
        {
            _nemesis = new NemesisHub(new IPEndPoint(_agentConfig.Ip, _agentConfig.SendPort),
                new IPEndPoint(_agentConfig.Ip, _agentConfig.ReceivePort));
            _nemesis.EnableEncryption(_xmlFileKeyStore);

            LoadKeys();

            _nemesis.CommandReceived += _nemesis_CommandReceived;
        }

        private void _nemesis_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            JsonRpcResponse response;

            try
            {
                var req = JsonRpcRequest.FromJsonString(e.Command);
                _log.Debug("Got message of type \"{0}\"... ", req.Method);

                response = ExecuteRequest(e, req);

            }
            catch(Exception x)
            {
                response = new JsonRpcResponse()
                {
                    Error = new JsonRpcError()
                    {
                        Code = JsonRpcErrorCode.ParseNotWellFormed,
                        Message = x.Message,
                        Data = x
                    }
                };
            }

            e.ResultSource.SetResult(JsonRpcDefaults.Encoding.GetString(response.Serialize()));
        }

        private JsonRpcResponse ExecuteRequest(CommandReceivedEventArgs e, JsonRpcRequest request)
        {
            JsonRpcResponse response = new JsonRpcResponse(request);

            if (request.Method == "heartbeat")
            {
                response = RpcHeartbeat(e.NodeId, request);
            }
            else if (request.Method == "getAgentConfig")
            {
                response = RpcGetAgentConfig(e.NodeId, request);
            }
            else if (request.Method == "uploadResults")
            {
                response = RpcUploadResults(e.NodeId, request);
            }
            else
            {
                response.Error = new JsonRpcError()
                {
                    Code = JsonRpcErrorCode.ServerRequestedMethodNotFound,
                    Message = "Requested method not found."
                };
            }
            return response;
        }

        private JsonRpcResponse RpcUploadResults(Guid nodeId, JsonRpcRequest req)
        {
            if (!_hostGuidsToIds.ContainsKey(nodeId))
                return new JsonRpcResponse(req)
                {
                    Error = new JsonRpcError()
                    {
                        Code = JsonRpcErrorCode.ServerInvalidMethodParameters
                    }
                };

            try {
                var received = new List<Guid>();

                var firstParam = (JToken)req.Params[0];
                var records = firstParam.ToObject<ResultRecordChunk>();

                _log.Info($"Got {records.Count} records from node {nodeId.ToString()}.");
                foreach (var kvpRecord in records)
                {
                    Guid guid = kvpRecord.Key;
                    ModuleResultRecord record = ((JObject)kvpRecord.Value).ToObject<ModuleResultRecord>();

                    _log.Debug($"{guid.ToString()} {record.Module}[{record.FunctionType}]{record.Function}: {record.Results.Count} target(s).");

                    if (record.FunctionType == ModuleFunctionType.Check)
                    {
                        foreach (var kvpResult in record.Results)
                        {
                            var _tl = new TraceLogger("Result");
                            var target = kvpResult.Key;
                            var result = ((JObject)kvpResult.Value).ToObject<CheckResult>();
                            _log.Debug($" - {target} => {result.RawValues.Count} raws, S:{(result.RanSuccessfully ? 1 : 0)} W:{(result.Warning ? 1 : 0)} C:{(result.Critical ? 1 : 0)} {result.Message}");
                            _tl.Trace("Init");

                            Result r = ResolveToResultEntity(target, record, result, nodeId);
                            _tl.Trace("Resolve");

                            _results.Create(r);
                            _tl.Trace("Insert");
                        }
                    }
                    else
                    {
                        _log.Warn($"Module function type {record.FunctionType} is not supported!");
                    }

                    received.Add(guid);
                }

                return new JsonRpcResponse(req)
                {
                    Result = received.ToArray()
                };
            }
            catch(Exception x)
            {
                return FetchApplicationErrorResponse(req, x);
            }
        }

        private Result ResolveToResultEntity(string target, IModuleResultRecord record, ModuleResult moduleResult, Guid nodeId)
        {
            var _tl = new TraceLogger("Result::Resolve");
            Host host = ResolveHost(nodeId);
            _tl.Trace("Host");
            var module = ResolveModule(record);
            _tl.Trace("Module");
            Function function = ResolveModuleFunction(record, module);
            _tl.Trace("Function");
            Result result = ConvertToResult(record, moduleResult, host, function, module, target);
            _tl.Trace("Result");

            return result;
        }

        private Entities.Modules.Module ResolveModule(IModuleResultRecord record)
        {
            return _modules.Get(new Entities.Modules.Module(record.Module));
        }

        private Function ResolveModuleFunction(IModuleResultRecord record, Entities.Modules.Module module)
        {
            int ft = 0;

            switch (record.FunctionType)
            {
                case ModuleFunctionType.Action: ft = FunctionType.Action; break;
                case ModuleFunctionType.Check: ft = FunctionType.Check; break;
                case ModuleFunctionType.Info: ft = FunctionType.Info; break;
                default: throw new Exception("Unknown function type");
            }

            return _functions.Get(new Function()
            {
                Name = record.Function,
                ModuleId = module.Id,
                Type = ft
            });

        }

        private Result ConvertToResult(IModuleResultRecord record, ModuleResult moduleResult,
            Host host, Function function, Entities.Modules.Module module, string target)
        {
            //var check = (Check)function;                 //      \
            //var checkDetail = (CheckDetail)detail;       // Hack: Until database has support for other function types
            //var checkResult = (CheckResult)moduleResult; //      /
            Result result = new Result()
            {
                HostId = host.Id,
                FunctionId = function.Id,
                ModuleId = module.Id,

                Exception = (moduleResult.ExecutionException != null) ? moduleResult.ExecutionException.Message : "",
                                                                    //CheckException does not contain the same fields as the Core Entity.
                ExecutionTime = record.CompletionTime,
                Message = moduleResult.Message,
                //ResultType = GetCheckResultType(checkResult), // Hack: Fix when GitHub Issue #3 in Hale-Lib is resolved.
                Target = target

            };
            return result;
        }

        private Host ResolveHost(Guid guid)
        {
            return _hosts.Get(new Host()
            {
                Guid = guid
            });
        }

        private int GetCheckResultType(CheckResult result)
        {
            if (result.RanSuccessfully)
            {
                if (result.Critical)
                    return (int)Status.Error;
                else if (result.Warning)
                    return (int) Status.Warning;
                else
                    return (int) Status.Ok;
            }
            else
            {
                return (int)Status.Error;
            }
            
        }

        private static JsonRpcResponse FetchApplicationErrorResponse(JsonRpcRequest req, Exception x)
        {
            return new JsonRpcResponse(req)
            {
                Error = new JsonRpcError()
                {
                    Code = JsonRpcErrorCode.ApplicationError,
                    Message = x.Message
                }
            };
        }

        private JsonRpcResponse RpcGetAgentConfig(Guid nodeId, JsonRpcRequest req)
        {
            throw new NotImplementedException();
        }

        private JsonRpcResponse RpcCheckResults(Guid nodeId, JsonRpcRequest req)
        {
            throw new NotImplementedException();
        }

        private JsonRpcResponse RpcHeartbeat(Guid nodeId, JsonRpcRequest req)
        {
            if (!_hostGuidsToIds.ContainsKey(nodeId))
                return new JsonRpcResponse(req)
                {
                    Error = new JsonRpcError()
                    {
                        Code = JsonRpcErrorCode.ServerInvalidMethodParameters
                    }
                };
            Host host = _hosts.Get(new Host() { Id = _hostGuidsToIds[nodeId] });
            host.Status = (int)Status.Ok;

            _hosts.UpdateStatus(host);

            return new JsonRpcResponse(req)
            {
                Result = "OK"
            };
        }

        public void GenerateRsaKeys()
        {
            var hosts = _hosts.List();

            ValidateHostKeyDirectory(_agentKeyStorePath);
            hosts.ForEach(host =>
            {
                CreateKeystore(_agentKeyStorePath, host);
            });
        }

        private void CreateKeystore(string hostKeysPath, Host host)
        {
            // Hack: Stores agent keys as XML as well as in database for easier development @fixme @security -NM
            // Todo: Write a proper keystore with database as sole backend
            var xfks = new XMLFileKeyStore(Path.Combine(hostKeysPath, host.HostName + ".xml"), true);
            if (!xfks.Available) // Only write to disk and database if new keys has been generated. -NM
            {
                xfks.Save();
            }
            if(host.RsaKey == null) // Hack: Since we dont want to generate new keys even if the database has been cleaned right now -NM
            {
                host = _hosts.Get(host);
                host.RsaKey = xfks.PrivateKey.Key;

                _hosts.UpdateRsa(host);
            }
        }

        private static void ValidateHostKeyDirectory(string hostKeysPath)
        {
            if (!Directory.Exists(hostKeysPath))
                Directory.CreateDirectory(hostKeysPath);
        }

        internal RSAKey PublicKey
        {
            get
            {
                return _nemesis.KeyStore.PublicKey;
            }
        }

        public void LoadKeys()
        {
            var hosts = _hosts.List();

            hosts.ForEach(host =>
            {
                // Todo: Handle concurrent dictionary better -NM
                _nemesis.NodesPublicKeys.TryAdd(host.Guid, new RSAKey { Key = host.RsaKey });
                _hostGuidsToIds.Add(host.Guid, host.Id);
            });
            
        }
    }


}
