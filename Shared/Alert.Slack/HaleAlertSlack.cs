using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HaleLib.Modules;
using HaleLib.Modules.Alerts;
using HaleLib.Modules.Checks;
using Newtonsoft.Json;

namespace Hale.Alerts
{
    public class HaleAlertPushbullet: Module, IAlertProvider
    {
        public new string Name { get; } = "Slack";

        public new string Author { get; } = "Hale Project";

        public override string Identifier { get; } = "com.itshale.core.slack";

        public new Version Version { get; } = new Version(0, 1, 1);

        public override string Platform { get; } = "Windows";

        public new decimal TargetApi { get; } = 1.3M;

        public Dictionary<string, ModuleFunction> Functions { get; set; } = new Dictionary<string, ModuleFunction>();

        public AlertFunctionResult DefaultAlert(AlertSettings settings)
        {
            AlertFunctionResult functionResult = new AlertFunctionResult();

            functionResult.RanSuccessfully = true;

            foreach (string target in settings.Targets)
            {
                AlertResult result = new AlertResult();
                try
                {
                    Dictionary<string, string> targetSettings = settings.TargetSettings[target];
                    HaleAlertSlackRecipient recipient = CreateRecipient(targetSettings);

                    SlackMessage msg = CreateSlackMessage(recipient, settings);
                    SendSlackMessage(msg, recipient);

                    result.RanSuccessfully = true;
                    result.Message = "OK";
                }
                catch (Exception x)
                {
                   functionResult.RanSuccessfully = result.RanSuccessfully = false;
                   functionResult.FunctionException = result.ExecutionException = x;
                }
                functionResult.AlertResults.Add(target, result);
            }

            

            return functionResult;
        }

        private static HaleAlertSlackRecipient CreateRecipient(Dictionary<string, string> targetSettings)
        {
            try
            {
                return new HaleAlertSlackRecipient()
                {
                    Webhook =  GetProperty("webhook", targetSettings, true),
                    Username = GetProperty("username", targetSettings, true),
                    Icon = GetProperty("icon", targetSettings),
                };
            }
            catch
            {
                throw new Exception("Malformatted configuration key");
            }
        }

        private static string GetProperty(string keyName, Dictionary<string, string> targetSettings, bool mandatory = false)
        {
            if (!targetSettings.ContainsKey(keyName))
                throw new Exception("Key for \"" + keyName + "\" is missing.");

            if (mandatory && String.IsNullOrEmpty(targetSettings[keyName]))
                throw new Exception("Value for mandatory key \"" + keyName + "\" is missing.");

            return targetSettings[keyName];
        }

        private void SendSlackMessage(SlackMessage msg, HaleAlertSlackRecipient recipient)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.UploadString(recipient.Webhook, "POST", JsonConvert.SerializeObject(msg));
                }
            }
            catch
            {
                throw new Exception("Could not connect to the Slack Server. Check your settings.");
            }
        }

        private SlackMessage CreateSlackMessage(HaleAlertSlackRecipient recipient, AlertSettings settings)
        {
                SlackMessage msg = new SlackMessage()
                {
                    Username = recipient.Username
                ,
                    Markdown = true
                ,
                    Icon = recipient.Icon
                ,
                    Pretext = settings.Message
                ,
                    Fields = new List<SlackField>()
                {
                    new SlackField("Node", settings.SourceHost, false),
                    new SlackField("Check", settings.SourceFunction, false)
                }
                ,
                    Color = GetAlertLevel(settings)
                };
                return msg;
        }

        private string GetAlertLevel(AlertSettings settings)
        {
            switch (settings.SourceCheckLevel)
            {
             case CheckLevel.Critical:
                    return "#ab2a2a";
                case CheckLevel.Warning:
                    return "#e47a0a";
                case CheckLevel.OK:
                    return "#019477";
                default:
                    return "#efefef";
            }
        }

        private void CreateSlackMessage()
        {
        }

        public void InitializeAlertProvider(AlertSettings settings)
        {
            this.AddAlertFunction(DefaultAlert);
        }
    }

    internal class SlackMessage
    {
        public SlackMessage()
        {
            Fields = new List<SlackField>();
        }

        [JsonProperty("color")]
        public string Color
        {
            get; set;

        }
        [JsonProperty("username")]
        public string Username
        {
            get; set;

        }

        [JsonProperty("icon_emoji")]
        public string Icon
        {
            get; set;

        }

        [JsonProperty("text")]
        public string Text
        {
            get; set;

        }

        [JsonProperty("fields")]
        public List<SlackField> Fields
        {
            get; set;
        }

        [JsonProperty("mrkdwn")]
        public bool Markdown { get; set; }

        [JsonProperty("pretext")]
        public string Pretext { get; set; }
    }

    public class SlackField
    {
        public SlackField(string title, string value, bool sh)
        {
            Title = title;
            Value = value;
            Short = sh;
        }

        [JsonProperty("title")]
        public string Title;
        [JsonProperty("value")]
        public string Value;
        [JsonProperty("short")]
        public bool Short;
    }

    public class HaleAlertSlackRecipient
    {
        public string Webhook { get; set; }
        public string Username { get; set; }
        public string Icon { get; set; }
    }
}
