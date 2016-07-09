using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale_Core.Config
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static AgentSection Agent (this Configuration config)
        {
            return config.Sections["agent"] as AgentSection;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ApiSection Api(this Configuration config)
        {
            return config.Sections["api"] as ApiSection;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static DatabaseSection Database(this Configuration config)
        {
            return config.Sections["database"] as DatabaseSection;
        }
    }
}
