using System;
using System.ComponentModel;
using System.Threading;
using Hale.Agent.Config;
using Hale.Lib.JsonRpc;
using NLog;
using Piksel.Nemesis;

namespace Hale.Agent.Communication
{
    internal class NemesisHeartbeatWorker
    {
        private NemesisConfig config;
        private BackgroundWorker worker;
        private NemesisNode node;
        

        public NemesisHeartbeatWorker(NemesisConfig config, NemesisNode node)
        {
            this.config = config;
            this.node = node;
        }

        public void Start()
        {
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(DoWork);
            worker.RunWorkerAsync(config);
        }
        public void DoWork(object sender, DoWorkEventArgs e)
        {
            var _config = (NemesisConfig)e.Argument;
            var _worker = (BackgroundWorker)sender;
            var _lastRun = DateTime.MinValue;

            ILogger _log = LogManager.GetLogger("NemesisHeartbeat");

            while (true)
            {
                if (!_worker.CancellationPending)
                {
                    if (DateTime.Now - _lastRun > config.HeartBeatInterval)
                    {
                        _log.Debug("Sending heartbeat to Core...");
                        _lastRun = DateTime.Now;

                        var req = new JsonRpcRequest()
                        {
                            Method = "heartbeat"
                        };

                        try
                        {
                            var res = node.SendCommand(JsonRpcDefaults.Encoding.GetString(req.Serialize()));
                            _log.Debug("Got response: {0}", res.Result);
                        }
                        catch (Exception x)
                        {
                            _log.Error("Error when sending heartbeat: {0}", x.Message);
                        }

                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
        }
        public void Stop()
        {
            worker.CancelAsync();
        }
    }
}
