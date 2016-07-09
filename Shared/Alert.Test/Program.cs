using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hale.Alert.Pushbullet;

namespace Hale.Alert
{
    class Program
    {


        public static void _(string format, params object[] args)
        {
            _(String.Format(format, args));
        }

        public static void _(string text)
        {
            var timeString = DateTime.Now.ToString("[yyyy-MM-dd HH:mm] ");
            Console.WriteLine(string.Concat(timeString, text));
        }

        static void Main(string[] args)
        {
            // tests for smtp
            //TestSmtp();

            // tests for pushbullet
            TestPushbullet();


            // 

            Console.Write("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static void TestPushbullet()
        {
            var config = new Dictionary<string, string>()
            {
                //{ "pb_access_token", "" },
            };

            var receps = new IHaleAlertRecipient[]
            {
                new HaleAlertPushbulletRecipient()
                {
                    Id = Guid.NewGuid(),
                    Name = "test",
                    Target = "nils.masen@gmail.com",
                    TargetType = PushbulletPushTarget.Email,
                    AccessToken = "o.A3d9D70dw7B54yDSs21paIOvCQr3tjEO"
                }
            };

            try
            {
                _("Creating Pushbullet Hale Alert instance...");
                var pbAlert = new HaleAlertPushbullet();

                _(" - Name: {0}, Version: {1}, Target API: {2}", pbAlert.Name,
                    pbAlert.Version, pbAlert.TargetApi);

                _("Initializing...");
                pbAlert.Initialize(config);

                _("Init response:\n" + pbAlert.InitResponse);

                _("Sending test push...");
                pbAlert.Send("Test message", "piksel-embla.HaleAlertTest", receps);

                _("Done!");
            }
            catch (Exception x)
            {
                _("Error: " + x.Message + (x.InnerException != null ? "; " + x.InnerException.Message : ""));
            }
        }

        private static void TestSmtp()
        {
            var config = new Dictionary<string, string>()
            {
                { "smtp_host", "nmail1.ballou.se" },
                { "smtp_port", "2525" },
            };

            try
            {
                _("Creating Email Hale Alert instance...");
                var emailAlert = new HaleAlertEmail();

                _(" - Name: {0}, Version: {1}, Target API: {2}", emailAlert.Name,
                    emailAlert.Version, emailAlert.TargetApi);

                _("Initiializing...");
                emailAlert.Initialize(config);

                _("Init response:\n" + emailAlert.InitResponse);

                _("Done!");
            }
            catch (Exception x)
            {
                _("Error: " + x.Message + (x.InnerException != null ? "; " + x.InnerException.Message : ""));
            }
        }
    }
}
