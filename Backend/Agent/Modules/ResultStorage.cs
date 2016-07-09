using Agent.Config;
using Agent.Core;
using HaleLib.Modules;
using HaleLib.Modules.Actions;
using HaleLib.Modules.Checks;
using HaleLib.Modules.Info;
using HaleLib.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Modules
{
    // Todo: Rename class @todo -NM
    class ResultStorage: IResultStorage, IDisposable
    {
        string _resultsPath;
        string _queuePath;
        ILogger _log = LogManager.GetLogger("ResultStorage");

        public ResultStorage()
        {
            var env = ServiceProvider.GetService<EnvironmentConfig>();
            _resultsPath = env.ResultsPath;

            //Load();
        }

        public void Dispose()
        {
            Persist();
        }

        Queue<IModuleResultRecord> _records = new Queue<IModuleResultRecord>();



        public void StoreResult(IModuleResultRecord record)
        {
            _records.Enqueue(record);
        }

        public void Persist()
        {
            var encoding = new UTF8Encoding(false);
            //var formatter = new BinaryFormatter();
            var js = new Newtonsoft.Json.JsonSerializer();
            while (_records.Count > 0)
            {
                var resultFile = Path.Combine(_resultsPath, Guid.NewGuid().ToString("N") + ".json");
                var record =_records.Dequeue();
                using (var fs = File.OpenWrite(resultFile))
                {
                    var sw = new StreamWriter(fs, encoding);
                    js.Serialize(sw, record);
                    sw.Flush();
                }
            }
        }

        public ResultRecordChunk Fetch(int maxRecords)
        {
            var encoding = new UTF8Encoding(false);
            var records = new ResultRecordChunk();

            var js = new Newtonsoft.Json.JsonSerializer();

            foreach (var file in Directory.GetFiles(_resultsPath)) {
                if (records.Count >= maxRecords) break;

                var guid = Guid.Parse(Path.GetFileNameWithoutExtension(file));
                try {
                    using (var fs = File.OpenRead(file))
                    {
                        var sr = new StreamReader(fs, encoding);
                        var jtr = new JsonTextReader(sr);
                        JToken token = JObject.Load(jtr);
                        var functiontype = (ModuleFunctionType)((int)token.SelectToken("FunctionType"));
                        var jr = token.CreateReader();
                        ModuleResultRecord record = null;
                        if (functiontype == ModuleFunctionType.Check)
                            record = js.Deserialize<CheckResultRecord>(jr);
                        if (functiontype == ModuleFunctionType.Info)
                            record = js.Deserialize<InfoResultRecord>(jr);
                        if (functiontype == ModuleFunctionType.Action)
                            record = js.Deserialize<ActionResultRecord>(jr);
                        records.Add(guid, record);
                    }
                }
                catch (Exception x)
                {
                    _log.Warn(x, $"Could not open file \"{file}\": {x.Message}");
                }

            }

            return records;
        }

        public void Clear(Guid[] uploaded)
        {
            foreach(var guid in uploaded)
            {
                var file = Path.Combine(_resultsPath, guid.ToString("N") + ".json");
                if(File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            
        }
    }
}
