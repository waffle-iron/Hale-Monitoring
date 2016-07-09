using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Config
{
    class EnvironmentConfig
    {
        string _dataPath;
        public string DataPath {
            get { return _dataPath; }
            set { _dataPath = AffirmPath(value); }
        }

        string _resultsPath;
        public string ResultsPath
        {
            get { return _resultsPath; }
            set { _resultsPath = AffirmPath(value); }
        }

        string _checksPath;
        public string ModulePath
        {
            get { return _checksPath; }
            set { _checksPath = AffirmPath(value); }
        }

        string _configFile;
        public string ConfigFile
        {
            get { return _configFile; }
            set { _configFile = AffirmFilePath(value); }
        }

        string _nemesisConfigFile;
        public string NemesisConfigFile
        {
            get { return _nemesisConfigFile; }
            set { _nemesisConfigFile = AffirmFilePath(value); }
        }

        string _nemesisKeyFile;
        public string NemesisKeyFile
        {
            get { return _nemesisKeyFile; }
            set { _nemesisKeyFile = AffirmFilePath(value); }
        }

        public static string AffirmPath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public static string AffirmFilePath(string file)
        {
            Path.GetDirectoryName(file);
            return file;
        }
    }
}
