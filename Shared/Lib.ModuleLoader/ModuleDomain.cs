using Hale.Lib.Modules;
using Hale.Lib.Modules.Actions;
using Hale.Lib.Modules.Alerts;
using Hale.Lib.Modules.Checks;
using Hale.Lib.Modules.Info;
using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using HaleModule = Hale.Lib.Modules.Module;

namespace Hale.Lib.ModuleLoader
{
    class ModuleDomain: MarshalByRefObject
    {
        ILogger _log;

        public void InitLogging()
        {
            var appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            LogManager.Configuration = new XmlLoggingConfiguration(Path.Combine(appPath, "nlog.config"));

            _log = LogManager.GetLogger(AppDomain.CurrentDomain.FriendlyName);
            _log.Debug($"AppDomain Sandbox for \"{AppDomain.CurrentDomain.FriendlyName}\" created.");
        }

        public T GetProvider<T>(string dll, out HaleModule module) where T: class, IModuleProviderBase
        {
            var providerInterface = typeof(T);

            var assembly = Assembly.LoadFile(dll);

            Type type = null;
            foreach (var t in assembly.ExportedTypes)
            {
                if (providerInterface.IsAssignableFrom(t))
                {
                    type = t;
                    break;
                }
            }

            if (type == null)
            {
                module = null;
                return null;
            }

            var instance = Activator.CreateInstance(type);
            var provider = (T)instance;
            module = (HaleModule)instance;

            _log.Debug($"Loaded module <{VersionedIdentifier.ToString(module)}> {module.Name}");

            return provider;
        }

        private ModuleFunctionResult _addVersionInfo(ModuleFunctionResult result, HaleModule module)
        {
            if(result.Module == null)
            {
                result.Module = new VersionedIdentifier(module);
            }
            return result;
        }

        public CheckFunctionResult ExecuteCheckFunction(string dll, string name, CheckSettings settings)
        {
            InitLogging();
            HaleModule module;
            var check = GetProvider<ICheckProvider>(dll, out module);
            check.InitializeCheckProvider(settings);
            var result = check.ExecuteCheckFunction(name, settings);
            return _addVersionInfo(result, module) as CheckFunctionResult;
        }


        public ActionFunctionResult ExecuteActionFunction(string dll, string name, ActionSettings settings)
        {
            InitLogging();
            HaleModule module;
            var action = GetProvider<IActionProvider>(dll, out module);
            action.InitializeActionProvider(settings);
            var result = action.ExecuteActionFunction(name, settings);
            return _addVersionInfo(result, module) as ActionFunctionResult;
        }


        public InfoFunctionResult ExecuteInfoFunction(string dll, string name, InfoSettings settings)
        {
            InitLogging();
            HaleModule module;
            var info = GetProvider<IInfoProvider>(dll, out module);
            info.InitializeInfoProvider(settings);
            var result = info.ExecuteInfoFunction(name, settings);
            return _addVersionInfo(result, module) as InfoFunctionResult;
        }

        public AlertFunctionResult ExecuteAlertFunction(string dll, string name, AlertSettings settings)
        {
            InitLogging();
            HaleModule module;
            var alert = GetProvider<IAlertProvider>(dll, out module);
            alert.InitializeAlertProvider(settings);
            var result = alert.ExecuteAlertFunction(name, settings);
            return _addVersionInfo(result, module) as AlertFunctionResult;
        }

        public ModuleRuntimeInfo GetModuleInfo(string dll)
        {
            InitLogging();
            var dict = new Dictionary<ModuleFunctionType, List<string>>(4);
            var checkList = new List<string>();
            var infoList = new List<string>();
            var actionList = new List<string>();
            var alertList = new List<string>();

            dict.Add(ModuleFunctionType.Check, checkList);
            dict.Add(ModuleFunctionType.Info, infoList);
            dict.Add(ModuleFunctionType.Action, actionList);
            dict.Add(ModuleFunctionType.Alert, alertList);


            HaleModule checkModule;
            var check = GetProvider<ICheckProvider>(dll, out checkModule);
            if (check != null)
            {
                check.InitializeCheckProvider(new CheckSettings());
                foreach (var name in check.Functions.Keys)
                {
                    if (name.IndexOf("check_") == 0)
                        checkList.Add(name.Substring("check_".Length));
                }
            }


            HaleModule infoModule;
            var info = GetProvider<IInfoProvider>(dll, out infoModule);
            if (info != null)
            {
                info.InitializeInfoProvider(new InfoSettings());
                foreach (var name in info.Functions.Keys)
                {
                    if (name.IndexOf("info_") == 0)
                        infoList.Add(name.Substring("info_".Length));
                }
            }

            HaleModule actionModule;
            var action = GetProvider<IActionProvider>(dll, out actionModule);
            if (action != null)
            {
                action.InitializeActionProvider(new ActionSettings());
                foreach (var name in action.Functions.Keys)
                {
                    if (name.IndexOf("action_") == 0)
                        actionList.Add(name.Substring("action_".Length));
                }
            }

            HaleModule alertModule;
            var alert = GetProvider<IAlertProvider>(dll, out alertModule);
            if (alert != null)
            {
                alert.InitializeAlertProvider(new AlertSettings());
                foreach (var name in alert.Functions.Keys)
                {
                    if (name.IndexOf("alert_") == 0)
                        alertList.Add(name.Substring("alert_".Length));
                }
            }

            // Todo: handle no valid function providers -NM 2016-01-19
            HaleModule module = checkModule ?? infoModule ?? actionModule ?? alertModule;

            return new ModuleRuntimeInfo() { Functions = dict, Module = new VersionedIdentifier(module) };
        }

    }
}
