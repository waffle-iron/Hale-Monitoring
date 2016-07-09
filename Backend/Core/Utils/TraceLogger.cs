using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale_Core.Utils
{
    public class TraceLogger
    {
        ILogger _log;
        DateTime _last;

        public TraceLogger(string name)
        {
#if DEBUG
            _log = LogManager.GetLogger("TL:" + name);
            _last = DateTime.Now;
#endif
        }

        public void Trace(string label)
        {
#if DEBUG
            _log.Trace(label + ": " + (DateTime.Now - _last).ToString("ss\\.ffff"));
            _last = DateTime.Now;
#endif
        }

        public void Reset()
        {
#if DEBUG
            _last = DateTime.Now;
#endif
        }
    }
}
