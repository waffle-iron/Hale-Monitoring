using Hale_Core.Contexts;
using HaleLib.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale_Core.Handlers
{
    partial class CoreScheduler
    {
        private void createDistPackages()
        {
            var distHandler = ServiceProvider.GetService<AgentDistHandler>();
            if (distHandler == null)
                return;
            distHandler.CreatePackages();
        }
    }
}
