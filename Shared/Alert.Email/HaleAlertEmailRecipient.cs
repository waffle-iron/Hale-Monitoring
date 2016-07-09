using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Hale.Alert
{
    class HaleAlertEmailRecipient: IHaleAlertRecipient
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
    }
}
