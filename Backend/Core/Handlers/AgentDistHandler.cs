using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using WindowsInstaller;
using Hale_Core.Entities;
using NLog;
using Piksel.Nemesis.Security;
using Hale_Core.Config;
using Hale_Core.Entities.Nodes;
using HaleLib.Utilities;
using System.Configuration;

namespace Hale_Core.Handlers
{
    class AgentDistHandler
    {
        // As these are low level variables, they shouldn't be changed during runtime.
        private readonly string _distPath;
        private readonly RSAKey _publicKey;
        private readonly string _msifile = "HaleAgent.msi";

        readonly ILogger _log = LogManager.GetLogger("AgentDistHandler");
        private readonly ushort _coreSendPort;
        private readonly ushort _coreReceivePort;
        private readonly string _coreHostname;

        private readonly string _platform = "platforms";
        private string target = "win32_i386"; // Todo: platform is @hardcoded for now @fixme -NM
        private readonly string _versionFile = "version.txt";

        public AgentDistHandler()
        {
            // Todo: Move service provider calls to where the values are actually used and avoid critical
            var env = ServiceProvider.GetServiceCritical<EnvironmentConfig>();
            var agentHandler = ServiceProvider.GetServiceCritical<AgentHandler>();
            var agentConfig = ServiceProvider.GetServiceCritical<Configuration>().Agent();

            _publicKey = agentHandler.PublicKey;
            _distPath = env.AgentDistPath;
            _coreSendPort = (ushort)agentConfig.SendPort;
            _coreReceivePort = (ushort)agentConfig.ReceivePort;
            _coreHostname = agentConfig.Hostname;
        }

        internal void CreatePackages(bool force = false)
        {
            var hostsContext = ServiceProvider.GetService<Contexts.Hosts>();
            if (hostsContext == null) return;
            var hosts = hostsContext.List();

            // NM: @todo @performance @blocking make this threaded?
            foreach(Host host in hosts)
            {
                CreatePackage(host, force);
            }
        }

        // Thanks Microsoft!
        MsiError _updateProperty(int hMsi, string key, string value)
        {
            int hView;
            //var me = MsiInterop.MsiDatabaseOpenView(hMsi, "UPDATE Property SET Value = ? WHERE Property = ?", out hView);
            var me = MsiInterop.MsiDatabaseOpenView(hMsi, "INSERT INTO Property (Value, Property) VALUES (?, ?)", out hView);
            if (me != MsiError.Success) return me;

            var hRec = MsiInterop.MsiCreateRecord(2);
            me = MsiInterop.MsiRecordSetString(hRec, 2, key);
            if (me != MsiError.Success) return me;
            me = MsiInterop.MsiRecordSetString(hRec, 1, value);
            if (me != MsiError.Success) return me;

            me = MsiInterop.MsiViewExecute(hView, hRec);
            if (me != MsiError.Success)
            {
                int errorRec = MsiInterop.MsiGetLastErrorRecord();
                uint errSize = 0;
                var sb = new StringBuilder();
                MsiInterop.MsiFormatRecord(0, errorRec, sb, ref errSize);
                sb = new StringBuilder((int)errSize);
                MsiInterop.MsiFormatRecord(0, errorRec, sb, ref errSize);
                _log.Error("Database query error: {0}", sb.ToString());

                errSize = 0;
                sb = new StringBuilder();
                MsiInterop.MsiFormatRecord(0, hRec, sb, ref errSize);
                sb = new StringBuilder((int)errSize);
                MsiInterop.MsiFormatRecord(0, hRec, sb, ref errSize);
                _log.Info("Record: {0}", sb.ToString());
            }
            return me;
        }

        private void CreatePackage(Host host, bool force = false)
        {
            var hostDistPath = Path.Combine(_distPath, host.Guid.ToString());

            if (force || !PackageIsLatest(hostDistPath))
            {

                _log.Info("Creating dist package for host {0}...", host.Guid.ToString());

                try
                {
                    CleanHostDistPath(hostDistPath);

                    string buildFile;
                    string outFile = CopyDistFiles(hostDistPath, out buildFile);

                    var hMsi = OpenMsiDatabase(buildFile);
                    SetAgentGuid(host, hMsi);
                    SetAgentKeys(host, hMsi);
                    SetCoreKey(hMsi);
                    SetCoreHostname(hMsi);
                    SetCorePorts(hMsi);

                    string nemesisConfig = GenerateNemesisConfig(host);
                    SetNemesisConfig(hMsi, nemesisConfig);
                    CommitToMsiDatabase(hMsi);

                    CloseMsiDatabase(hMsi);
                    MsiInterop.MsiCloseAllHandles();
                    RenameOutputFile(outFile, buildFile);
                }
                catch (Exception e)
                {
                    _log.Debug(e.Message);
                }
            }
            else
            {
                _log.Debug($"Skipping dist package creation for host {host.Guid.ToString()}...");
            }

        }

        private bool PackageIsLatest(string hostDistPath)
        {
            var hostVerstionPath = Path.Combine(hostDistPath, _versionFile);

            var templateVersionPath = Path.Combine(_distPath, _platform, target, _versionFile);
            if (!File.Exists(templateVersionPath))
                GenerateTemplateVersionFile(templateVersionPath);
            if (!File.Exists(hostVerstionPath))
                return false;
            return File.ReadAllText(hostVerstionPath) == File.ReadAllText(templateVersionPath);
        }

        private void GenerateTemplateVersionFile(string templateVersionPath)
        {
            var versionKey = "ProductVersion";
            uint size = 255;

            var templateMsi = Path.Combine(_distPath, _platform, target, _msifile);
            var hMsi = OpenMsiPackage(templateMsi);

            StringBuilder sb = new StringBuilder((int)size);

            var me = MsiInterop.MsiGetProperty(hMsi, versionKey, sb, ref size);

            if (me == MsiError.MoreData)
            {
                size++; // Note: Adding space for terminating null character -NM
                sb = new StringBuilder((int)size);
                me = MsiInterop.MsiGetProperty(hMsi, versionKey, sb, ref size);
            }

            if(me == MsiError.Success)
            {
                File.WriteAllText(templateVersionPath, sb.ToString());
            }
            else
            {
                throw new Exception($"MSI Exception {me.ToString()}");
            }
        }

        private string CopyDistFiles(string hostDistPath, out string buildFile)
        {
            var outFile = Path.Combine(hostDistPath, _msifile);
            buildFile = outFile + ".build";

            _log.Debug("Copying dist files...");

            var tempPath = Path.Combine(_distPath, _platform, target);

            File.Copy(Path.Combine(tempPath, _msifile), buildFile);
            File.Copy(Path.Combine(tempPath, _versionFile), Path.Combine(hostDistPath, _versionFile));

            return outFile;
        }

        private void CleanHostDistPath(string hostDistPath)
        {
            _log.Debug("Cleaning output directory {0}...", hostDistPath);
            if (Directory.Exists(hostDistPath))
            {
                try
                {
                    Directory.Delete(hostDistPath, true);
                }
                catch (Exception x)
                {
                    throw new Exception($"Could not clean output directory. Got exception: {x.Message}. Skipping package...");
                }
            }
            var di = Directory.CreateDirectory(hostDistPath);
            var waitSeconds = 10;
            while (!di.Exists)
            {
                waitSeconds -= 1;

                if (waitSeconds == 0)
                {
                    throw new Exception("Timed out waiting for the OS to create our folder.");
                }
                Thread.Sleep(1000);
            }
        }

        private void RenameOutputFile(string outFile, string buildFile)
        {
            _log.Debug("Renaming output file...");
            File.Move(buildFile, outFile);
        }

        private void CloseMsiDatabase(int hMsi)
        {
            _log.Debug("Closing MSI database...");
            MsiError me = MsiInterop.MsiCloseHandle(hMsi);
            if (me != MsiError.Success)
                throw new Exception(message: $"Error: {me.ToString()}");
        }

        private void CommitToMsiDatabase(int hMsi)
        {
            _log.Debug("Commiting changes to MSI database...");
            MsiError me = MsiInterop.MsiDatabaseCommit(hMsi);
            if (me != MsiError.Success)
                throw new Exception(message: $"Error: {me.ToString()}");
        }

        private void SetNemesisConfig(int hMsi, string nemesisConfig)
        {
            
            _log.Debug("Setting nemesis config...");
            MsiError me = _updateProperty(hMsi, "HALE_AGENT_NEMESIS_CONFIG", nemesisConfig);

            if (me != MsiError.Success)
                throw new Exception(message: $"Error: {me.ToString()}");
        }

        private string GenerateNemesisConfig(Host host)
        {
            string nemesisConfig;
            string configPath = Path.Combine(_distPath, "common", "nemesis.yaml");

            using (var sr = File.OpenText(configPath))
            {
                nemesisConfig = sr.ReadToEnd();
            }

            nemesisConfig = nemesisConfig.Replace("<HOSTNAME>", _coreHostname);
            nemesisConfig = nemesisConfig.Replace("<SENDPORT>", _coreReceivePort.ToString()); // Note: We swap the send/receive values here -NM
            nemesisConfig = nemesisConfig.Replace("<RECEVEPORT>", _coreSendPort.ToString());
            nemesisConfig = nemesisConfig.Replace("<GUID>", host.Guid.ToString());

            return nemesisConfig;
        }

        private void SetCorePorts(int hMsi)
        {
            
            _log.Debug("Setting core port...");
            MsiError me = _updateProperty(hMsi, "HALE_CORE_SEND_PORT", _coreSendPort.ToString());
            if (me != MsiError.Success)
                throw new Exception(message: $"Error: {me.ToString()}");
            me = _updateProperty(hMsi, "HALE_CORE_RECEIVE_PORT", _coreReceivePort.ToString());
            if (me != MsiError.Success)
                throw new Exception(message: $"Error: {me.ToString()}");
        }

        private void SetCoreHostname(int hMsi)
        {
            _log.Debug("Setting core hostname...");
            MsiError me = _updateProperty(hMsi, "HALE_CORE_HOSTNAME", _coreHostname);

            if (me != MsiError.Success)
                throw new Exception(message: $"Error: {me.ToString()}");
        }

        private void SetCoreKey(int hMsi)
        {
            
            _log.Debug("Setting core key...");
            var xmlKeyCore = RSA.ExportToXml(_publicKey.Key, false);
            MsiError me = _updateProperty(hMsi, "HALE_CORE_KEY", xmlKeyCore);

            if (me != MsiError.Success)
                throw new Exception(message: $"Error: {me.ToString()}");
        }

        private void SetAgentKeys(Host host, int hMsi)
        {
            
            _log.Debug("Setting agent keys...");
            var xmlKeyAgent = RSA.ExportToXml(host.RsaKey);
            MsiError me = _updateProperty(hMsi, "HALE_AGENT_KEYS", xmlKeyAgent);

            if (me != MsiError.Success)
                throw new Exception(message: $"Error: {me.ToString()}");
            
        }

        private void SetAgentGuid(Host host, int hMsi)
        {
            _log.Debug("Setting agent GUID...");
            MsiError me = _updateProperty(hMsi, "HALE_AGENT_GUID", host.Guid.ToString());

            if (me != MsiError.Success)
                throw new Exception(message: $"Error: {me.ToString() }");
            
        }

        private int OpenMsiDatabase(string msiFile, MsiDbPersistMode mode = MsiDbPersistMode.Transact)
        {

            int hMsi;

            _log.Debug("Opening MSI database...");
            MsiError me = MsiInterop.MsiOpenDatabase(msiFile, mode, out hMsi);
            if (me != MsiError.Success)
                throw new Exception(message: $"Error: {me.ToString()}");

            return hMsi;
        }

        private int OpenMsiPackage(string msiFile)
        {

            int hMsi;

            _log.Debug("Opening MSI database...");
            MsiError me = MsiInterop.MsiOpenPackage(msiFile, out hMsi);
            if (me != MsiError.Success)
                throw new Exception(message: $"Error: {me.ToString()}");

            return hMsi;
        }
    }
}
