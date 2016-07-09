using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale_Core.Entities.Modules
{
    public class Function
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public int ModuleId { get; set; }
    }

    public class FunctionType
    {
        public int Id { get; set; }
        public string Type { get; set; }

        public static int Action = 1;
        public static int Check = 2;
        public static int Info = 3;
    }
}
