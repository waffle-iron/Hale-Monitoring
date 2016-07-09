using System.Configuration;

namespace Hale_Core.Config
{
    public class ApiSection: ConfigurationSection
    {
        [ConfigurationProperty("host")]
        public string Host
        {
            get
            {
                return (string)this["host"];
            }
            set
            {
                this["host"] = value;
            }
        }

        [ConfigurationProperty("port")]
        public int Port
        {
            get
            {
                return (int)this["port"];
            }
            set
            {
                this["port"] = value;
            }
        }

        [ConfigurationProperty("scheme")]
        public string Scheme
        {
            get
            {
                return (string)this["scheme"];
            }
            set
            {
                this["scheme"] = value;
            }
        }

        public static void ValidateSection(Configuration _config)
        {
            if (_config.Sections["api"] == null)
            {
                var section = new ApiSection
                {
                    Port = 8989,
                    Host = "+",
                    Scheme = "http"
                };
                _config.Sections.Add("api", section);
            }
        }
    }
}
