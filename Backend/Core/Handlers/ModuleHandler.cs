using Hale.Core.Contexts;
using Hale.Lib.ModuleLoader;
using Hale.Lib.Modules;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;

namespace Hale.Core.Handlers
{
    class ModuleHandler
    {
        ILogger _log = LogManager.GetLogger("ModuleHandler");

        public void ScanForModules(string scanPath)
        {
            _log.Info($"Scanning path \"{scanPath}\"...");
            foreach(var file in scanDir(scanPath))
            {
                var shortPath = file.Replace(scanPath, "");
                _log.Debug($" - Found manifest at \"{shortPath}\".");
                var mi = GetModuleInfo(Path.GetDirectoryName(file));
                if(mi != null)
                    AddModule(mi);
            }
        }

        private void AddModule(ModuleInfo mi)
        {
            _log.Info($"Adding module <{mi}>, {mi.ActionFunctions.Count} action-, {mi.InfoFunctions.Count} info- and {mi.CheckFunctions.Count} check-functions.");

            var me = mi.GetModuleEntity();

            try {
                var modules = new Modules();
                modules.Create(me);
            }
            catch (Exception x)
            {
                _log.Warn($"Could not add module <{mi}> to database: {x.Message}");
                return;
            }

            var moduleFunctions = new ModuleFunctions();

            foreach (var fn in mi.ActionFunctions) {
                try
                {
                    var mf = new Entities.Modules.Function();
                    mf.Type = Entities.Modules.FunctionType.Action;
                    mf.Name = fn;
                    mf.ModuleId = me.Id;
                    moduleFunctions.Create(mf);
                }
                catch (Exception x)
                {
                    _log.Warn($"Could not add module function <{mi}>::{fn} to database: {x.Message}");
                    return;
                }
            }

            foreach (var fn in mi.CheckFunctions)
            {
                try
                {
                    var mf = new Entities.Modules.Function();
                    mf.Type = Entities.Modules.FunctionType.Check;
                    mf.Name = fn;
                    mf.ModuleId = me.Id;
                    moduleFunctions.Create(mf);
                }
                catch (Exception x)
                {
                    _log.Warn($"Could not add module function <{mi}>::{fn} to database: {x.Message}");
                    return;
                }
            }

            foreach (var fn in mi.InfoFunctions)
            {
                try
                {
                    var mf = new Entities.Modules.Function();
                    mf.Type = Entities.Modules.FunctionType.Info;
                    mf.Name = fn;
                    mf.ModuleId = me.Id;
                    moduleFunctions.Create(mf);
                }
                catch (Exception x)
                {
                    _log.Warn($"Could not add module function <{mi}>::{fn} to database: {x.Message}");
                    return;
                }
            }

        }

        private string[] scanDir(string scanPath)
        {
            List<string> files = new List<string>();
            foreach (var dir in Directory.GetDirectories(scanPath))
            {
                files.AddRange(Directory.GetFiles(dir, "manifest.yaml"));
                files.AddRange(scanDir(dir));
            }
            return files.ToArray();
        }

        public ModuleInfo GetModuleInfo(string modulePath)
        {
            var manifestPath = Path.Combine(modulePath, "manifest.yaml");
            var yd = new YamlDotNet.Serialization.Deserializer(
                namingConvention: new CamelCaseNamingConvention());
            
            try
            {
                _log.Debug($"Parsing manifest {manifestPath}...");
                ModuleInfo mi;
                using (var fs = File.OpenRead(manifestPath))
                {
                    var sr = new StreamReader(fs);
                    var manifest = yd.Deserialize<ModuleManifest>(sr);
                    mi = ModuleInfo.FromManifest(manifest);
                }
                mi.ModulePath = modulePath;
                _log.Debug($"Retrieving runtime info from \"{mi.Manifest.Module.Filename}\"...");
                mi.GetRuntimeInfo();
                return mi;
            }
            catch (Exception x)
            {
                _log.Warn($"Error parsing manifest.yaml: {x.Message}");
            }
            return null;
        }
    }

    class ModuleInfo
    {
        public Version Version
        {
            get { return Module.Version; }
            set { Module.Version = value; }
        }

        public string Identifier
        {
            get { return Module.Identifier; }
            set { Module.Identifier = value; }
        }

        public override string ToString()
        {
            return Module.ToString();
        }

        public string ModulePath;

        public List<string> CheckFunctions = new List<string>();
        public List<string> InfoFunctions = new List<string>();
        public List<string> ActionFunctions = new List<string>();

        public bool NeedsRuntimeInfo = true;
        public bool Valid = false;

        public ModuleManifest Manifest;

        /// <summary>
        ///  The version specified inside the manifest, or if missing, the module dll version
        /// </summary>
        public VersionedIdentifier Module;

        /// <summary>
        ///  The version of the module dll
        /// </summary>
        public VersionedIdentifier Runtime;

        public static ModuleInfo FromManifest(ModuleManifest manifest)
        {
            var mi = new ModuleInfo();
            mi.Manifest = manifest;
            if (!string.IsNullOrEmpty(manifest.Information.Identifier) && manifest.Information.Version != null)
                mi.Module = new VersionedIdentifier(manifest.Information.Identifier,
                    manifest.Information.Version);
            return mi;
        }

        public void GetRuntimeInfo()
        {
            var runtimePath = Path.Combine(ModulePath, Manifest.Module.Filename);
            if (File.Exists(runtimePath))
            {
                var mri = ModuleLoader.GetModuleInfo(Manifest.Module.Filename, ModulePath);
                Runtime = mri.Module;
                if (Module == null)
                    Module = mri.Module;
                CheckFunctions = mri.Functions[ModuleFunctionType.Check];
                InfoFunctions = mri.Functions[ModuleFunctionType.Info];
                ActionFunctions = mri.Functions[ModuleFunctionType.Action];
                NeedsRuntimeInfo = false;
                Valid = true;
            }
            else
            {
                Valid = false;
                throw new FileNotFoundException($"Module runtime file \"{runtimePath}\" not found.");
            }
        }

        public Entities.Modules.Module GetModuleEntity()
        {
            return new Entities.Modules.Module() { Version = Version, Identifier = Identifier };
        }
    }

    enum ModuleArchiveCompression
    {
        Lzma, Deflate, Bzip2
    }
}
