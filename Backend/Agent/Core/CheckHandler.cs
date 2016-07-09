using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Hale.Lib.Modules.Checks;

namespace Hale.Agent.Core
{
    internal class CheckHandler  
    {
        List<dynamic> _checks;


        internal CheckHandler()
        {
            LoadChecks();
        }


        public void ReloadChecks()
        {
            try { 
                _checks = null;
                LoadChecks();
            }
            catch
            {
                throw new Exception("Failed to reload the checks");
            }
        }
        private void LoadChecks()
        {
            _checks = new List<dynamic>();
            foreach (var dll in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "checks", "*.dll"))
            {

                Assembly assembly = Assembly.LoadFile(dll);
                Type type = assembly.GetExportedTypes().First(x => x.IsSubclassOf(typeof(ICheckProvider)) /* x.FullName == "Hale.Agent.Check" */);
                dynamic c = Activator.CreateInstance(type);
                _checks.Add(c);

            }
        }

        public ICheckProvider GetCheck(string name)
        {
            foreach (var c in _checks)
            {

                string output = c.Name.ToString();
                if (c.Name == name)
                {
                    return c;
                }
            }

            return null;
        }

        public List<string> ListChecks()
        {
            return _checks.Select(c => c.Name).Cast<string>().ToList();
        }
    }
}
