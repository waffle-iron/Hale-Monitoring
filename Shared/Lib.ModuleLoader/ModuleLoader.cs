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
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Hale.Lib.ModuleLoader
{
    public class ModuleLoader : MarshalByRefObject
    {

        static ModuleDomain GetDomain(string dll, string checkPath)
        {
            AppDomainSetup adSetup = new AppDomainSetup();

            adSetup.ApplicationBase = Path.GetFullPath(checkPath);
            PermissionSet permSet = new PermissionSet(PermissionState.Unrestricted);
            AppDomain newDomain = AppDomain.CreateDomain("Module::" + Path.GetFileNameWithoutExtension(dll), null, adSetup, permSet);

            ObjectHandle handle = Activator.CreateInstanceFrom(
                newDomain, typeof(ModuleDomain).Assembly.ManifestModule.FullyQualifiedName,
                typeof(ModuleDomain).FullName
            );

            return (ModuleDomain)handle.Unwrap();
        }

        public static CheckFunctionResult ExecuteCheckFunction(string dll, string modulePath, string name, CheckSettings settings)
        {
            ModuleDomain moduleDomain = GetDomain(dll, modulePath);
            return moduleDomain.ExecuteCheckFunction(Path.GetFullPath(Path.Combine(modulePath, dll)), name, settings);
        }

        public static InfoFunctionResult ExecuteInfoFunction(string dll, string modulePath, string name, InfoSettings settings)
        {
            ModuleDomain moduleDomain = GetDomain(dll, modulePath);
            return moduleDomain.ExecuteInfoFunction(Path.GetFullPath(Path.Combine(modulePath, dll)), name, settings);
        }

        public static ActionFunctionResult ExecuteActionFunction(string dll, string modulePath, string name, ActionSettings settings)
        {
            ModuleDomain moduleDomain = GetDomain(dll, modulePath);
            return moduleDomain.ExecuteActionFunction(Path.GetFullPath(Path.Combine(modulePath, dll)), name, settings);
        }

        public static AlertFunctionResult ExecuteAlertFunction(string dll, string modulePath, string name, AlertSettings settings)
        {
            ModuleDomain moduleDomain = GetDomain(dll, modulePath);
            return moduleDomain.ExecuteAlertFunction(Path.GetFullPath(Path.Combine(modulePath, dll)), name, settings);
        }

        public static ModuleRuntimeInfo GetModuleInfo(string dll, string modulePath)
        {
            ModuleDomain moduleDomain = GetDomain(dll, modulePath);
            return moduleDomain.GetModuleInfo(Path.GetFullPath(Path.Combine(modulePath, dll)));
        }

    }
}
