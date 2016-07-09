using System;
using System.ServiceProcess;
using System.IO;
using Hale.Agent.Communication;
using NLog;
using Hale.Agent.Config;
using Hale.Agent.Modules;
using Hale.Lib.Utilities;

namespace Hale.Agent
{
    public partial class HaleAgentService : ServiceBase
    {
        private ILogger _log = LogManager.GetLogger("Service");

        private Config.AgentConfig config;
        private NemesisController nemesis;
        private AgentScheduler scheduler;
        private IResultStorage resultStorage;
        private EnvironmentConfig env;

        public HaleAgentService()
        {
            InitializeComponent();
        }

#if DEBUG
        internal void StartDebug()
        {
            OnStart(new string[] { "" });
            
        }
#endif

        protected override void OnStart(string[] args)
        {
            env = new EnvironmentConfig();
            env.DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Hale", "Agent");
            ServiceProvider.SetService<EnvironmentConfig>(env);

            InitializeNemesis();

            //UpdateConfiguration();

            LoadConfiguration();
            InitializeResultStorage();
            InitializeScheduler();
        }

        private void InitializeResultStorage()
        {
            _log.Info("Initializing Result Storage...");
            env.ResultsPath = Path.Combine(env.DataPath, "Results");

            resultStorage = new ResultStorage();
            ServiceProvider.SetService(resultStorage);
        }

        private void InitializeNemesis()
        {
            _log.Info("Initializing Nemesis...");
            env.NemesisConfigFile = Path.Combine(env.DataPath, "nemesis.yaml");
            env.NemesisKeyFile = Path.Combine(env.DataPath, "agent-keys.xml");
            nemesis = new NemesisController();
            ServiceProvider.SetService(nemesis);
        }


        private void UpdateConfiguration()
        {
            _log.Info("Updating configuration...");
            var config = nemesis.RetrieveString("getAgentConfig");
        }

        private void LoadConfiguration()
        {
            _log.Debug("Loading configuration...");
            env.ConfigFile = Path.Combine(env.DataPath, "config.yaml");
            if(!File.Exists(env.ConfigFile))
            {
                _log.Warn("No configuration file has been fetched.");
                return;
            }
            config = Config.AgentConfig.LoadFromFile(env.ConfigFile);
            ServiceProvider.SetService(config);

            _log.Debug("Configuration successfully loaded, found {0} checks.", config.Checks.Count);
        }

        private void InitializeScheduler()
        {
            _log.Info("Initializing Scheduler...");
            scheduler = new AgentScheduler();
            scheduler.Start();
        }

        protected override void OnStop()
        {
            scheduler.Stop();
            nemesis.Stop();
        }
    }
}
