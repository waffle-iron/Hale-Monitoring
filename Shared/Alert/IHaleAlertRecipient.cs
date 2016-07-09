using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Hale.Alert
{
    public interface IHaleAlertRecipient
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}
