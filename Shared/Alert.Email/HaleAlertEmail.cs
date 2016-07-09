using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using Config = System.Collections.Generic.Dictionary<string, string>; 

namespace Hale.Alert
{
    public class HaleAlertEmail: IHaleAlert
    {
        public string Name { get { return "Email"; } }

        readonly Version _version = new Version(0, 1, 1);
        public Version Version { get { return _version; }  }

        public decimal TargetApi { get { return 0.1M; } }

        public bool Ready { get; private set; }

        private Config _config;

        SmtpClient smtp;

        public void Initialize(Config config)
        {
            _config = config;
            try
            {
                var useSsl = (_config.ContainsKey("smtp_ssl") && bool.Parse(_config["smtp_ssl"]));
                var smtpHost = _config["smtp_host"];
                var smtpPort = int.Parse(_config["smtp_port"]);

                if (!useSsl)
                {
                    string response;
                    if (!SmtpHelper.TestConnection(smtpHost, smtpPort, out response))
                    {
                        InitResponse = response;
                        throw new HaleAlertInitializeException(new Exception("Cannot verify SMTP connection."));
                    }

                    InitResponse = response;
                }

                smtp = new SmtpClient(smtpHost, smtpPort) {EnableSsl = useSsl};


                Ready = true;
            }
            catch (Exception x)
            {
                throw new HaleAlertInitializeException(x);
            }

        }

        public string InitResponse { get; private set; }

        public void Send(string message, string source, IHaleAlertRecipient[] recipients)
        {

        }
    }
}
