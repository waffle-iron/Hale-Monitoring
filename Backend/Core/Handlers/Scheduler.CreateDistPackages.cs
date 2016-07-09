using Hale.Lib.Utilities;

namespace Hale.Core.Handlers
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
