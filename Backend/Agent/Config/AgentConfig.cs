using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Globalization;
using HaleLib.Modules.Checks;
using HaleLib.Modules.Info;
using HaleLib.Modules.Actions;
using System.Collections;

namespace Agent.Config
{
    class AgentConfig
    {
        [YamlMember( Alias ="checks")]
        public Dictionary<string, Dictionary<string, object>> _checks { get; set; }
            = new Dictionary<string, Dictionary<string, object>>();

        [YamlIgnore]
        public Dictionary<string, CheckSettings> Checks { get; set; }

        [YamlMember(Alias = "info")]
        public Dictionary<string, Dictionary<string, object>> _info { get; set; }
            = new Dictionary<string, Dictionary<string, object>>();

        [YamlIgnore]
        public Dictionary<string, InfoSettings> Info { get; set; }

        [YamlMember(Alias = "actions")]
        public Dictionary<string, Dictionary<string, object>> _actions { get; set; }
            = new Dictionary<string, Dictionary<string, object>>();

        [YamlIgnore]
        public Dictionary<string, ActionSettings> Actions { get; set; }

        public Dictionary<string, AgentConfigModule> Modules { get; set; }
        public Dictionary<string, AgentConfigTask> Tasks { get; set; }

        static readonly NumberFormatInfo nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };

        public static AgentConfig LoadFromFile(string file)
        {
            AgentConfig ac;

            using (StreamReader sr = File.OpenText(file))
            {
                Deserializer ds = new Deserializer(namingConvention: new CamelCaseNamingConvention());
                ac = ds.Deserialize<AgentConfig>(sr);
            }

            CheckAgentConfigForNull(ac);
            ac = InitializeAgentConfigLists(ac);

            ac = SetDefaultTaskValues(ac);

            if (ac._checks != null)
                ac = LoadSerializedCheck(ac);

            if (ac._info != null)
                ac = LoadSerializedInfo(ac);

            if (ac._actions != null)
                ac = LoadSerializedActions(ac);

            return ac;
        }

        private static AgentConfig InitializeAgentConfigLists(AgentConfig ac)
        {
            ac.Checks = new Dictionary<string, CheckSettings>();
            ac.Info = new Dictionary<string, InfoSettings>();
            ac.Actions = new Dictionary<string, ActionSettings>();

            return ac;
        }

        private static AgentConfig SetDefaultTaskValues(AgentConfig ac)
        {
            if (!ac.Tasks.ContainsKey("persistResults"))
            {
                ac.Tasks.Add("persistResults", new AgentConfigTask()
                {
                    Enabled = true,
                    Interval = new TimeSpan(0, 10, 0),
                    Startup = false
                });
            }
            if (!ac.Tasks.ContainsKey("uploadResults"))
            {
                ac.Tasks.Add("uploadResults", new AgentConfigTask()
                {
                    Enabled = true,
                    Interval = new TimeSpan(0, 30, 0),
                    Startup = false
                });
            }
            if (!ac.Tasks.ContainsKey("sendHeartbeat"))
            {
                ac.Tasks.Add("sendHeartbeat", new AgentConfigTask()
                {
                    Enabled = true,
                    Interval = new TimeSpan(0, 10, 0),
                    Startup = true
                });
            }
            return ac;
        }

        private static AgentConfig LoadSerializedActions(AgentConfig ac)
        {
            foreach (var _action in ac._actions)
            {
                var actionProperties = GetProperties(_action.Value);
                var actionSettings = new ActionSettings(actionProperties);
                actionSettings.TargetSettings = new Dictionary<string, Dictionary<string, string>>();
                var targetSettings = new Dictionary<string, string>();
                foreach (var target in GetTargets(_action.Value))
                    actionSettings.TargetSettings.Add(target.Key, targetSettings);
                actionSettings.ParseRaw();
                ac.Actions.Add(_action.Key, actionSettings);
            }

            return ac;
        }

        private static AgentConfig LoadSerializedInfo(AgentConfig ac)
        {
            foreach (var _info in ac._info)
            {
                var infoProperties = GetProperties(_info.Value);
                var infoSettings = new InfoSettings(infoProperties);
                infoSettings.TargetSettings = new Dictionary<string, Dictionary<string, string>>();
                var targetSettings = new Dictionary<string, string>();
                foreach (var target in GetTargets(_info.Value))
                    infoSettings.TargetSettings.Add(target.Key, targetSettings);
                infoSettings.ParseRaw();
                ac.Info.Add(_info.Key, infoSettings);
            }

            return ac;
        }

        private static AgentConfig LoadSerializedCheck(AgentConfig ac)
        {
            foreach (var _check in ac._checks)
            {
                var checkProperties = GetProperties(_check.Value);
                var checkSettings = new CheckSettings(checkProperties);
                checkSettings.TargetSettings = new Dictionary<string, Dictionary<string, string>>();
                checkSettings.Actions = GetActions(_check.Value);
                checkSettings.Thresholds = GetThresholds(_check.Value);
                var targetSettings = new Dictionary<string, string>();
                foreach (var target in GetTargets(_check.Value))
                    checkSettings.TargetSettings.Add(target.Key, targetSettings);
                checkSettings.ParseRaw();
                ac.Checks.Add(_check.Key, checkSettings);
            }
            return ac;
        }

        private static void CheckAgentConfigForNull(AgentConfig ac)
        {
            if (ac == null)
            {
                throw new Exception("Could not deserialize config.yaml. Make sure the syntax is correct.");
            }
        }

        private static CheckThresholds GetThresholds(Dictionary<string, object> input)
        {
            if (input.ContainsKey("thresholds"))
            {
                var td = (IDictionary)input["thresholds"];
                return new CheckThresholds()
                {
                    Critical = Single.Parse(td["critical"].ToString(), nfi),
                    Warning = Single.Parse(td["warning"].ToString(), nfi)
                };
            }
            else
            {
                return new CheckThresholds() { Warning = 0.5F, Critical = 1.0F };
            }
        }

        private static CheckActions GetActions(Dictionary<string, object> input)
        {
            return (input.ContainsKey("actions") ?input["actions"] as CheckActions : new CheckActions());
        }

        static Dictionary<string, Dictionary<string, string>> GetTargets(Dictionary<string, object> input)
        {
            var targets = new Dictionary<string, Dictionary<string, string>>();
            if (input.ContainsKey("targets")) {
                foreach(DictionaryEntry kvpTarget in (IDictionary)input["targets"])
                {
                    Dictionary<string, string> targetSettings = new Dictionary<string, string>();
                    if(kvpTarget.Value != null)
                    {
                        foreach(DictionaryEntry kvpSettings in (IDictionary)kvpTarget.Value)
                        {
                            targetSettings.Add(kvpSettings.Key.ToString(), kvpSettings.Value.ToString());
                        }
                    }
                    targets.Add(kvpTarget.Key.ToString(), targetSettings);
                }
            }

            if (targets.Count == 0)
            {
                targets.Add("default", new Dictionary<string, string>());
            }
            return targets;
        }

        static Dictionary<string, string> GetProperties(Dictionary<string, object> input)
        {
            string[] ignoreKeys = new []{ "targets", "actions", "thresholds" };
            return input.Where(item => !ignoreKeys.Contains(item.Key))
                .ToDictionary(item => item.Key, item => item.Value as string);
        }
    }

    class AgentConfigTask
    {
        public bool Enabled { get; set; } = true;
        public TimeSpan Interval { get; set; }
        public bool Startup { get; set; } = false;
    }

    class AgentConfigModule
    {
        public string Dll { get; set; }
    }
}
