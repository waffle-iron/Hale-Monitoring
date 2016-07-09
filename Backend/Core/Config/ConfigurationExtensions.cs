using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale_Core.Config
{
    public static class ConfigurationExtensions
    {
        public static AgentSection Agent (this Configuration config)
        {
            return config.Sections["agent"] as AgentSection;
        }

        public static ApiSection Api(this Configuration config)
        {
            return config.Sections["api"] as ApiSection;
        }

        public static DatabaseSection Database(this Configuration config)
        {
            return config.Sections["database"] as DatabaseSection;
        }
    }
}
