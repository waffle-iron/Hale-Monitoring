using HaleLib.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale_Core.Entities.Modules
{
    public class Module
    {
        public int Id { get; set; }
        public string Identifier { get; set; }

        // Hack: Permits the dapper deserializer to correctly build this object -NM 2016-01-21
        public Version Version { get; set; }
        public int Major { set { updateVersion( major: value ); } }
        public int Minor { set { updateVersion( minor: value ); } }
        public int Revision { set { updateVersion( revision: value ); } }
        private int? _major, _minor, _revision;
        private void updateVersion (int? major = null, int? minor = null, int? revision = null)
        {
            if (major.HasValue) _major = major;
            if (minor.HasValue) _minor = minor;
            if (revision.HasValue) _revision = revision;

            if (_major.HasValue && _minor.HasValue && _revision.HasValue)
                Version = new Version(_major.Value, _minor.Value, 0, _revision.Value);
        }

        public Module() { }
        public Module(VersionedIdentifier vi)
        {
            Identifier = vi.Identifier;
            Version = vi.Version;
        }
    }
}
