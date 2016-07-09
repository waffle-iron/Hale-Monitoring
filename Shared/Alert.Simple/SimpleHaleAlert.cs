using HaleLib.Modules;
using HaleLib.Modules.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hale.Alerts
{
    public class SimpleHaleAlert : Module, IAlertProvider
    {
        public new string Name { get; } = "Simple Alert";

        public new string Author { get; } = "Hale Project";

        public override string Identifier { get; } = "com.itshale.core.simplealert";

        public new Version Version { get; } = new Version(0, 1, 1);

        public override string Platform { get; } = "Windows";

        public new decimal TargetApi { get; } = 1.3M;

        public Dictionary<string, ModuleFunction> Functions { get; set; } = new Dictionary<string, ModuleFunction>();

        public AlertFunctionResult DefaultAlert(AlertSettings settings)
        {
            var afr = new AlertFunctionResult();
            foreach (var target in settings.Targets) {
                var result = new AlertResult();
                var mbresult = MessageBox.Show(settings.Message, settings.SourceString, MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                result.RanSuccessfully = true;
                result.Message = $"{target} -> {mbresult.ToString()}";
                result.Target = target;
                afr.AlertResults.Add(target, result);
            }
            afr.RanSuccessfully = true;
            return afr;
        }

        public void InitializeAlertProvider(AlertSettings settings)
        {
            this.AddAlertFunction(DefaultAlert);
        }
    }
}
