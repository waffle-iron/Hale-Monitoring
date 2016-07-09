using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale_Core.Entities.Modules
{
    /// <summary>
    /// 
    /// </summary>
    public class Function
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int Type { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int ModuleId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FunctionType
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static int Action = 1;
        
        /// <summary>
        /// 
        /// </summary>
        public static int Check = 2;

        /// <summary>
        /// 
        /// </summary>
        public static int Info = 3;
    }
}
