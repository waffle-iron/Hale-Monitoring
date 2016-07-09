using System.Configuration;
using System.Net;

namespace Hale_Core.Config
{

    /// <summary>
    /// 
    /// </summary>
    public class AgentSection : ConfigurationSection
    {
        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_config"></param>
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
