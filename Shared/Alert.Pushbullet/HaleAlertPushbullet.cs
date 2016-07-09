using System;
using System.Collections.Generic;
using Hale.Lib.Modules;
using Hale.Lib.Modules.Alerts;
using Hale.Alert.Pushbullet;

namespace Hale.Alerts
{
    public class HaleAlertPushbullet: Module, IAlertProvider
    {
        public new string Name { get; } = "Pushbullet";

        public new string Author { get; } = "Hale Project";

        public override string Identifier { get; } = "com.itshale.core.pushbullet";

        public new Version Version { get; } = new Version(0, 1, 1);

        public override string Platform { get; } = "Windows";

        public new decimal TargetApi { get; } = 1.3M;

        public Dictionary<string, ModuleFunction> Functions { get; set; } = new Dictionary<string, ModuleFunction>();

        public bool Ready { get; private set; }

        public string InitResponse { get; private set; }

        private PushbulletApi _api;

        public AlertFunctionResult DefaultAlert(AlertSettings settings)
        {
            var afr = new AlertFunctionResult();
            foreach (var target in settings.Targets)
            {
                var ar = new AlertResult();
                try {
                    var targetSettings = settings.TargetSettings[target];
                    var recipient = new HaleAlertPushbulletRecipient()
                    {
                        AccessToken = targetSettings["accessToken"],
                        Target = targetSettings["target"],
                        TargetType = (PushbulletPushTarget)Enum.Parse(typeof(PushbulletPushTarget),
                            targetSettings["targetType"]),
                        Name = targetSettings["name"],
                        Id = Guid.NewGuid()

                    };
                    var response = _api.Push("Hale Alert", string.Format("{0}\n{1}", settings.SourceString, settings.Message), recipient);

                    ar.Message = response.Type;
                    ar.RanSuccessfully = true;

                }
                catch (Exception x)
                {
                    ar.RanSuccessfully = false;
                    ar.ExecutionException = x;
                }
                afr.AlertResults.Add(target, ar);
            }
            afr.RanSuccessfully = true;
            return afr;
        }

        public void InitializeAlertProvider(AlertSettings settings)
        {
            this.AddAlertFunction(DefaultAlert);
            _api = new PushbulletApi("");
        }
    }
}
