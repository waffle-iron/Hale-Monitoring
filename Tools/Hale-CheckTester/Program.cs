using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NLog;
using System.Reflection;
using CommandLine;
using CommandLine.Text;
using Hale.Lib.Modules.Checks;
using Hale.Lib.ModuleLoader;
using Hale.Lib.Modules.Info;
using Hale.Lib.Modules.Actions;
using Hale.Lib.Modules.Alerts;
using Hale.Lib.Modules;
using YamlDotNet.Serialization;

namespace Hale_CheckTester
{
    class Options
    {
        [Option('p', "module-path", Required = false,  HelpText = "Path to module directory")]
        public string ModulePath { get; set; }

        [Option('d', "dll", Required = true, HelpText = "DLL file name")]
        public string Dll { get; set; }

        [OptionList('c', "check-target", Separator = ',')]
        public List<string> CheckTargets { get; set; } = new List<string>();

        [OptionList('i', "info-target", Separator = ',')]
        public List<string> InfoTargets { get; set; } = new List<string>();

        [OptionList('a', "action-target", Separator = ',')]
        public List<string> ActionTargets { get; set; } = new List<string>();

        [OptionList('A', "alert-target", Separator = ',')]
        public List<string> AlertTargets { get; set; } = new List<string>();

        [ParserState]
        public IParserState LastParserState { get; set; }
    }

    class Program: MarshalByRefObject
    {
        static CheckSettings checkSettings;
        static Options _opts;

        static ILogger _log = LogManager.GetLogger("HaleModuleTester");

        static void Main(string[] args)
        {
            Console.Title = "Hale Module Tester";

            var v = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine($"Hale Module Tester v{v.Major}.{v.Minor} build {v.Build} release {v.Revision}, copyright (c) 20{v.Build / 1000}, Hale Project.\n");


            _opts = new Options();

            var parser = new Parser(config => config.HelpWriter = Console.Out);

            if (parser.ParseArguments(args, _opts))
            {
                if (String.IsNullOrEmpty(_opts.ModulePath))
                {
                    _opts.ModulePath = Path.GetDirectoryName(Path.GetFullPath(_opts.Dll));
                    _opts.Dll = Path.GetFileName(_opts.Dll);
                }

                if(_opts.CheckTargets.Count<1 && _opts.ActionTargets.Count<1 && _opts.InfoTargets.Count<1 && _opts.AlertTargets.Count<1)
                {
                    _opts.CheckTargets.Add("default:default");
                }


                if (_opts.CheckTargets.Count > 0)
                {
                    _log.Info("Running module check tasks:");
                    var checkTargets = ProcessTargets(_opts.CheckTargets);

                    foreach(var checkTarget in checkTargets) {
                        checkSettings = new CheckSettings();
                        checkSettings.Thresholds = new CheckThresholds() { Critical = 1.0F, Warning = 0.5F };
                        RunCheckTask(checkTarget.Key, checkTarget.Value, checkSettings);
                    }
                }
                
                if (_opts.InfoTargets.Count > 0)
                {
                    _log.Info("Running module info tasks:");

                    var infoTargets = ProcessTargets(_opts.InfoTargets);
                    foreach (var infoTarget in infoTargets)
                    {
                        var infoSettings = new InfoSettings();
                        RunInfoTask(infoTarget.Key, infoTarget.Value, infoSettings);
                    }
                }

                if (_opts.ActionTargets.Count > 0)
                {
                    _log.Info("Running module action tasks:");

                    var actionTargets = ProcessTargets(_opts.ActionTargets);
                    foreach (var actionTarget in actionTargets)
                    {
                        var actionSettings = new ActionSettings();
                        RunActionTask(actionTarget.Key, actionTarget.Value, actionSettings);
                    }
                }

                if (_opts.AlertTargets.Count > 0)
                {
                    _log.Info("Running module alert tasks:");

                    var alertTargets = ProcessTargets(_opts.AlertTargets);
                    foreach (var alertTarget in alertTargets)
                    {
                        var actionSettings = new AlertSettings();
                        RunAlertTask(alertTarget.Key, alertTarget.Value, actionSettings);
                    }
                }


            }
            else
            {

                Console.WriteLine("Error parsing command line arguments.");
                var help = new HelpText();
                if (_opts.LastParserState.Errors.Any())
                {
                    var errors = help.RenderParsingErrorsText(_opts, 2); // indent with two spaces

                    if (!string.IsNullOrEmpty(errors))
                    {
                        help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
                        help.AddPreOptionsLine(errors);
                    }
                }
                Console.WriteLine(help);
            }

            Console.Write("\nPress any key to exit...");
            Console.ReadKey();

        }

        private static Dictionary<string, List<string>> ProcessTargets (List<string> rawTargets)
        {

            var results = new Dictionary<string, List<string>>();

            foreach (var rawTarget in rawTargets)
            {
                string name = "default";
                string target = "default";
                if (rawTarget.IndexOf(':') >= 0)
                {
                    var targetParts = rawTarget.Split(':');
                    name = targetParts[0];
                    target = targetParts[1];
                }
                else
                {
                    target = rawTarget;
                }
                if (!results.ContainsKey(name))
                    results.Add(name, new List<string>());
                results[name].Add(target);
            }

            return results;
        }

        private static void RunTask<T> (string name, string type, object settings)
        {
            var checkName = Path.GetFileNameWithoutExtension(_opts.Dll);
            var taskName = $"{checkName}({name})";

            Console.Title = taskName + " - Hale Module Tester";

            _log.Info($"Executing {type} task {taskName}...");

            try
            {
                var added = DateTime.Now;

                var checkPath = _opts.ModulePath;
                var dll = Path.GetFullPath(Path.Combine(checkPath, _opts.Dll));

                if (!File.Exists(dll))
                {
                    throw new FileNotFoundException($"Module DLL \"{dll}\" not found!");
                }

                if (typeof(T) == typeof(CheckSettings))
                {
                    var result = ModuleLoader.ExecuteCheckFunction(_opts.Dll, _opts.ModulePath, name, settings as CheckSettings);
                    if (result == null) throw new Exception("Returned null result!");
                    if (!result.RanSuccessfully) throw result.FunctionException;
                    foreach (var cr in result.CheckResults)
                    {
                        _log.Info($"Check {taskName}:\"{cr.Key}\" returned raw values {cr.Value.RawValues}.");
                        _log.Info($"  -> {cr.Value.Message}");
                    }
                }
                else if (typeof(T) == typeof(InfoSettings))
                {
                    var result = ModuleLoader.ExecuteInfoFunction(_opts.Dll, _opts.ModulePath, name, settings as InfoSettings);
                    if (result == null) throw new Exception("Returned null result!");
                    if (!result.RanSuccessfully) throw result.FunctionException;
                    foreach (var ir in result.InfoResults)
                    {
                        _log.Info($"Info {taskName}:\"{ir.Key}\" returned info items {ir.Value.ItemsAsString()}.");
                        _log.Info($"  -> {ir.Value.Message}");
                    }
                }
                else if (typeof(T) == typeof(ActionSettings))
                {
                    var result = ModuleLoader.ExecuteActionFunction(_opts.Dll, _opts.ModulePath, name, settings as ActionSettings);
                    if (result == null) throw new Exception("Returned null result!");
                    if (!result.RanSuccessfully) throw result.FunctionException;
                    foreach (var ar in result.ActionResults)
                    {
                        _log.Info($"Action {taskName}:\"{ar.Key}\" executed.");
                        _log.Info($"  -> {ar.Value.Message}");
                    }
                }
                else if (typeof(T) == typeof(AlertSettings))
                {
                    var result = ModuleLoader.ExecuteAlertFunction(_opts.Dll, _opts.ModulePath, name, settings as AlertSettings);
                    if (result == null) throw new Exception("Returned null result!");
                    if (!result.RanSuccessfully) throw result.FunctionException;
                    foreach (var ar in result.AlertResults)
                    {
                        _log.Info($"Alert {taskName}:\"{ar.Key}\" executed.");
                        _log.Info($"  -> {ar.Value.Message}");
                    }
                }
                var completed = DateTime.Now;
                _log.Info($"The {type} task {taskName} completed in { (completed - added).TotalSeconds.ToString("F2")} second(s)");


            }
            catch (Exception x)
            {
                _log.Error($"Error running {type} task {taskName}: {x.Message}");
                if(x.InnerException != null)
                    _log.Error($" - Inner exception: {x.InnerException.Message}");
                _log.Warn("Stacktrace:\n" + x.StackTrace);
            }

        }

        private static void RunCheckTask(string name, List<string> targets, CheckSettings settings)
        {
            var ed = new Dictionary<string, string>();
            foreach (var target in targets)
            {
                settings.TargetSettings.Add(target, ed);
            }

            RunTask<CheckSettings>(name, "check", settings);
        }

        private static void RunInfoTask(string name, List<string> targets, InfoSettings settings)
        {
            var ed = new Dictionary<string, string>();
            foreach (var target in targets)
            {
                settings.TargetSettings.Add(target, ed);
            }

            RunTask<InfoSettings>(name, "info", settings);
        }

        private static void RunActionTask(string name, List<string> targets, ActionSettings settings)
        {
            var ed = new Dictionary<string, string>();
            foreach (var target in targets)
            {
                settings.TargetSettings.Add(target, ed);
            }

            RunTask<ActionSettings>(name, "action", settings);
        }

        private static void RunAlertTask(string name, List<string> targets, AlertSettings settings)
        {
            var yds = new Deserializer(namingConvention: new YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention());

            settings.Message = "Foo and Bar is Baz!";
            settings.SourceModule = new VersionedIdentifier("com.example.dummy", new Version(1, 0, 16054, 99));
            settings.SourceFunctionType = ModuleFunctionType.Check;
            settings.SourceFunction = "DummyCheck";
            settings.SourceTarget = "DummyTarget";

            var moduleConfig = Path.Combine(_opts.ModulePath, "alert_" + name + ".yaml");

            if (File.Exists(moduleConfig))
            {
                using (var sr = File.OpenText(moduleConfig))
                {
                    var conf = yds.Deserialize<Dictionary<string, string>>(sr);
                    foreach(var vkp in conf)
                    {
                        settings[vkp.Key] = vkp.Value;
                    }
                }
            }

            foreach (var target in targets)
            {
                var targetConfig = Path.Combine(_opts.ModulePath, $"alert_{name}_{target}.yaml");
                var targetSettings = new Dictionary<string, string>();
                if (File.Exists(targetConfig))
                {
                    using (var sr = File.OpenText(targetConfig))
                    {
                        var conf = yds.Deserialize<Dictionary<string, string>>(sr);
                        foreach (var vkp in conf)
                        {
                            targetSettings[vkp.Key] = vkp.Value;
                        }
                    }
                }
                settings.TargetSettings.Add(target, targetSettings);
            }

            RunTask<AlertSettings>(name, "alert", settings);
        }

    }
}
