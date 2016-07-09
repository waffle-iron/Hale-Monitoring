using System.Configuration;
using System.Net;

namespace Hale_Core.Config
{
    public class AgentSection : ConfigurationSection
    {
        [ConfigurationProperty("sendport")]
        public int SendPort
        {
            get
            {
                try
                {
                    return (int)this["sendport"];
                }
                catch
                {
                    return 8988;
                }
            }
            set
            {
                this["sendport"] = value;
            }
        }

        [ConfigurationProperty("receiveport")]
        public int ReceivePort
        {
            get
            {
                try
                {
                    return (int)this["receiveport"];
                }
                catch
                {
                    return 8987;
                }
            }
            set
            {
                this["receiveport"] = value;
            }
        }

        [ConfigurationProperty("hostname")]
        public string Hostname
        {
            get
            {
                try
                {
                    return (string)this["hostname"];
                }
                catch
                {
                    return "localhost";
                }
            }
            set
            {
                this["hostname"] = value;
            }
        }

        public IPAddress Ip
        {
            get
            {
                try {
                    return IPAddress.Parse(_rawIp);
                }
                catch
                {
                    return IPAddress.Loopback;
                }
            }
            set
            {
                _rawIp = value.ToString();
            }
        }

        [ConfigurationProperty("ip")]
        private string _rawIp
        {
            get
            {
                try
                {
                    return (string)this["ip"];
                }
                catch
                {
                    return IPAddress.Loopback.ToString();
                }
            }
            set
            {
                this["ip"] = value;
            }
        }

        public static void ValidateSection(Configuration _config)
        {
            if (_config.Sections["agent"] == null)
            {
                var section = new AgentSection
                {
                    SendPort = 8988,
                    ReceivePort = 8987,
                    Ip = IPAddress.Any,
                    Hostname = "localhost"
                };
                _config.Sections.Add("agent", section);
            }
        }
    }
}
