using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agent.Config;
using Piksel.Nemesis;
using Piksel.Nemesis.Security;
using NLog;
using System.ComponentModel;
using System.Threading;
using Newtonsoft.Json.Linq;
using Agent.Core;
using HaleLib.JsonRpc;
using HaleLib.Modules.Checks;
using HaleLib.Modules;
using HaleLib.Utilities;

namespace Agent.Communication
{
    class NemesisController
    {
        ILogger _log = LogManager.GetLogger("NemesisController");

        private NemesisConfig config;
        private readonly XMLFileKeyStore keystore;
        private readonly NemesisNode node;
        private readonly NemesisHeartbeatWorker heartbeatworker;

        public NemesisController()
        {
            var env = ServiceProvider.GetService<EnvironmentConfig>();

            _log.Debug("Loading nemesis config file \"{0}\"...", env.NemesisConfigFile);
            config = NemesisConfig.LoadFromFile(env.NemesisConfigFile);

            _log.Debug("Host: {0}, Send port: {1}, Receive port: {4}, Encryption: {2}, GUID: {3}", 
                config.Hostname, config.SendPort, config.UseEncryption, config.Id, config.ReceivePort);

            node = new NemesisNode(config.Id, new int[] { config.ReceivePort, config.SendPort }, config.Hostname, false);
            if (config.UseEncryption && string.Empty != env.NemesisKeyFile) {
                _log.Debug("Loading encryption keys from \"{0}\"...", env.NemesisKeyFile);
                keystore = new XMLFileKeyStore(env.NemesisKeyFile);

                var coreKeystore = new XMLFileKeyStore(env.NemesisKeyFile.Replace("agent", "core"));
                node.HubPublicKey = coreKeystore.PublicKey;

                node.EnableEncryption(keystore);

                _log.Debug("Encryption is enabled.");
            }

            node.Connect();

            //_log.Debug("Starting hearbeat worker thread...");
            //heartbeatworker = new NemesisHeartbeatWorker(config, node);
            //heartbeatworker.Start();

            
        }

        public void Stop()
        {
            heartbeatworker.Stop();
        }

        public string RetrieveString(string command, params object[] parameters)
        {
            var req = new JsonRpcRequest()
            {
                Method = command
            };

            if (parameters.Length > 0)
                req.Params = parameters;

            try
            {
                var respTask = node.SendCommand(JsonRpcDefaults.Encoding.GetString(req.Serialize()));
                var response = JsonRpcResponse.FromJsonString(respTask.Result); // Blocking!
                var result = (string) response.Result;
                _log.Debug("Got a {0} character response string.", result.Length);
                return result;
            }
            catch (Exception x)
            {
                _log.Error("Error when retrieving string: {0}", x.Message);
                return "";
            }
        }

        public string SendCommand(string command)
        {
            return RetrieveString(command, new object[0]);
        }

        public Guid[] UploadResults(ResultRecordChunk records)
        {
            var req = new JsonRpcRequest()
            {
                Method = "uploadResults",
                Params = new object[] { records }
            };
            try
            {
                _log.Info($"Uploading {records.Count} result records to Core...");
                var serialized = JsonRpcDefaults.Encoding.GetString(req.Serialize());
                var respTask = node.SendCommand(serialized);
                var response = JsonRpcResponse.FromJsonString(respTask.Result); // Blocking!
                if (response.Error != null)
                {
                    _log.Warn($"Error uploading records: {response.Error.Message} (0x{response.Error.Code.ToString("x")})");
                    return new Guid[0];
                }
                else
                {
                    var result = ((JToken)response.Result).ToObject<Guid[]>();
                    _log.Info($"Uploaded {result.Length} result records successfully.");
                    return result;
                }
            }
            catch (Exception x)
            {
                _log.Warn($"Error uploading records: {x.Message}");
                return new Guid[0];
            }
        }

        
    }

 
}
