using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Hale.Agent.Config
{
    class NemesisConfig
    {
        public string Hostname { get; set; }
        public ushort SendPort { get; set; }
        public ushort ReceivePort { get; set; }
        public bool UseEncryption { get; set; }
        public Guid Id { get; set; }
        public TimeSpan HeartBeatInterval { get; set; }

        public static NemesisConfig LoadFromFile(string file)
        {
            using (var sr = File.OpenText(file))
            {
                var ds = new Deserializer(namingConvention: new CamelCaseNamingConvention());
                return ds.Deserialize<NemesisConfig>(sr);
            }
        }
    }
}
