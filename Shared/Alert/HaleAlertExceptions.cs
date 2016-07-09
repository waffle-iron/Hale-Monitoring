using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale.Alert
{
    public class HaleAlertException: Exception
    {
        public HaleAlertException():
            base("Unknown Hale Alert exception.")
        {
        }

        public HaleAlertException(string message):
            base(message)
        {
        }

        public HaleAlertException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public HaleAlertException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class HaleAlertInitializeException : HaleAlertException
    {
        public HaleAlertInitializeException() :
            base("Canot initialize Hale Alert module.")
        {
        }

        public HaleAlertInitializeException(Exception innerException) :
            base("Canot initialize Hale Alert module.", innerException)
        {
        }
    }
}
