using Hale.Lib.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale.Lib.ModuleLoader
{
    [Serializable]
    public class ModuleRuntimeInfo
    {
        public Dictionary<ModuleFunctionType, List<string>> Functions;
        public VersionedIdentifier Module;
    }
}
