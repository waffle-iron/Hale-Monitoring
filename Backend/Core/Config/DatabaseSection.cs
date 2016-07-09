using System.Configuration;

namespace Hale_Core.Config
{
    /// <summary>
    /// 
    /// </summary>
    public class DatabaseSection: ConfigurationSection
    {
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = "mssql")]
        public string Type
        {
            get
            {
                return (string)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("database", DefaultValue = "HaleDB")]
        public string Database
        {
            get
            {
                return (string)this["database"];
            }
            set
            {
                this["database"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("user")]
        public string User
        {
            get
            {
                return (string)this["user"];
            }
            set
            {
                this["user"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("password")]
        public string Password
        {
            get
            {
                return (string)this["password"];
            }
            set
            {
                this["password"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("useIntegratedSecurity")]
        public bool UseIntegratedSecurity
        {
            get
            {
                return (bool)this["useIntegratedSecurity"];
            }
            set
            {
                this["useIntegratedSecurity"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_config"></param>
        public static void ValidateSection(Configuration _config)
        {
            if (_config.Sections["database"] == null)
            {
                var section = new DatabaseSection
                {
                    Host = "localhost",
                    Port = 1433,
                    Type = "mssql",
                    Database = "HaleDB",
                    User = "",
                    Password = "",
                    UseIntegratedSecurity = false
                };

                _config.Sections.Add("database", section);
            }
        }

    }
}
