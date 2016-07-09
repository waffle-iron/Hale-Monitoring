using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceProcess;
using Hale_Core.Config;
using Hale_Core.Contexts;
using Hale_Core.Handlers;
using NLog;
using HaleLib.Utilities;

namespace Hale_Core
{
    /// <summary>
    /// The main entry point for starting the Hale-Core service.
    /// </summary>
    public partial class HaleCoreService : ServiceBase
    {
        private readonly Logger _log;
        private AgentHandler _agentHandler;
        private Configuration _config;

        private EnvironmentConfig _env;

        /// <summary>
        /// Default constructor for the Hale-Core service
        /// </summary>
        public HaleCoreService()
        {
            InitializeComponent();

            _env = new EnvironmentConfig();
            _env.DataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Hale\Core\";
            _env.AgentDistPath = Path.Combine(_env.DataPath, "dist");
            _env.ConfigFile = Path.Combine(_env.DataPath, "HaleCore.config");
            ServiceProvider.SetService(_env);

            _log = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Method used for executing a threaded start of the core.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += ThreadedStart;
            worker.RunWorkerAsync();
        }

        private void ThreadedStart(object sender, DoWorkEventArgs args) 
        {
            
            _log.Info("Reading configuration...");

            AppConfig.Change(_env.ConfigFile);
            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Each sections defaults is set in its ValidateSection method -NM 2016-01-17
            ApiSection.ValidateSection(_config);
            DatabaseSection.ValidateSection(_config);
            AgentSection.ValidateSection(_config);
            SaveConfigToFileIfMissing();

            ServiceProvider.SetService(_config);

#if DEBUG
            //LaunchModuleHandler();
            LaunchCoreInstances();
#else
            LaunchCoreInstances();
#endif
        }

        private void LaunchModuleHandler()
        {
            ModuleHandler moduleHandler = new ModuleHandler();

            moduleHandler.ScanForModules(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                @"Hale\Agent\Modules\"));

        }

        private void LaunchCoreInstances()
        {
            LaunchSecurityHandler();

            // Todo: Decide if we are going to use the entity context together with the service proivder
            // and if so move this to a separate entity -NM 2016-01-17
            ServiceProvider.SetService<Contexts.Hosts>(new Hosts());

            LaunchApiHandler();
            LaunchAgentHandler();
            LaunchAgentDistributionHandler();
        }

        private void LaunchAgentHandler()
        {
            _log.Info("Creating Agent Handler instance...");
            
            _agentHandler = new AgentHandler();
            _agentHandler.GenerateRsaKeys();
            ServiceProvider.SetService(_agentHandler);
        }

        private void LaunchAgentDistributionHandler()
        {
            _log.Info("Creating Agent distribution handler...");


            // Simon@NM:
            //      If possible, I suggest that this gets simplified.
            //      Four params in the constructor is too much imho. @fixme
            // How about 0 params? -NM 2016-01-17
            AgentDistHandler distHandler = new AgentDistHandler();
            ServiceProvider.SetService(distHandler);

            _log.Info("Creating Agent dist packages...");
            distHandler.CreatePackages();
        }

        private void LaunchApiHandler()
        {
            _log.Info("Creating API Handler instance...");
            ApiHandler apiHandler = new ApiHandler();
        }

        private void LaunchSecurityHandler()
        {
            _log.Info("Creating Security Handler instance...");
            SecurityHandler securityHandler = new SecurityHandler();
        }

        private void SaveConfigToFileIfMissing()
        {
            if (!_config.HasFile)
            {
                _log.Warn("No configuration file present! Writing new file with defaults to \"{0}\".", _config.FilePath);
                _config.Save(ConfigurationSaveMode.Modified);
            }
            else
            {
                _log.Info("Loaded configuration from \"{0}\".", _config.FilePath);
            }
        }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        protected override void OnStop()
        {
        }

#if DEBUG
        /// <summary>
        /// Start in debugger mode, which enables us to start Hale-Core as a terminal app instead of a windows service, which is the default behaviour.
        /// </summary>
        public void DebugStart()
        {
            OnStart(new string[] { });
            while (true)
            {

            }
        }
#endif

    }
}
