using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class TraceLogger
    {
        ILogger _log;
        DateTime _last;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public TraceLogger(string name)
        {
#if DEBUG
            _log = LogManager.GetLogger("TL:" + name);
            _last = DateTime.Now;
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        public void Trace(string label)
        {
#if DEBUG
            _log.Trace(label + ": " + (DateTime.Now - _last).ToString("ss\\.ffff"));
            _last = DateTime.Now;
#endif
        }

        /// <summary>
        /// </summary>
        public void Reset()
        {
#if DEBUG
            _last = DateTime.Now;
#endif
        }
    }
}
