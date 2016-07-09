using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale_Core.Entities.Modules
{
    class Result
    {
        public int Id { get; set; }
        public int HostId { get; set; }
        public int ModuleId { get; set; }
        public int FunctionId { get; set; }
        public string Target { get; set; }
        public DateTime ExecutionTime { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }
}
