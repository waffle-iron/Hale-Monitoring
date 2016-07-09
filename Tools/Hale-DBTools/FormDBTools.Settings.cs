using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Hale_DBTools
{
    public partial class FormDBTools
    {
        private void LoadSettings()
        {
            var _s = Properties.Settings.Default;

            if (!_s.hasUpgraded)
            {
                _s.Upgrade();
                _s.hasUpgraded = true;
                _s.Save();
            }

            tbDbHost.Text = _s.dbHost;
            tbDbPassword.Text = _s.dbPassword;
            tbDbUsername.Text = _s.dbUsername;
            tbDbPort.Text = _s.dbPort.ToString();

            cbDbType.SelectedIndex = 0;
            cbDbDefault.Text = _s.dbDefault;

            if (_s.dbHost == "")
            {
                tabControl1.SelectedTab = tabSettings;
            }
        }

        private void bTestDb_Click(object sender, EventArgs e)
        {

            try
            {
                var sc = GetConnection(tbDbHost.Text, ushort.Parse(tbDbPort.Text), tbDbUsername.Text, tbDbPassword.Text);

                sc.Open();
                var databases = sc.Query<string>("SELECT name FROM sys.databases").ToArray();

                cbDbDefault.Items.Clear();
                foreach (string database in databases)
                {
                    cbDbDefault.Items.Add(database);
                }

                lDbConnectionResult.Text = String.Format("Connection result: OK! Found {0} database(s).", databases.Count());
            }
            catch (Exception x)
            {
                lDbConnectionResult.Text = String.Format("Connection result: Failed!\nReason: {0}", x.Message);
            }
        }

        private SqlConnection GetConnection(bool useDefaultDB = true)
        {
            var _s = Properties.Settings.Default;
            return GetConnection(_s.dbHost, _s.dbPort, _s.dbUsername, _s.dbPassword, useDefaultDB ? _s.dbDefault : null);
        }

        private SqlConnection GetConnection(string host, ushort port, string user, string password, string defaultDb = null)
        {
            SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
            scsb.DataSource = host + "," + port;
            scsb.IntegratedSecurity = false;
            if(defaultDb != null)
                scsb.InitialCatalog = defaultDb;

            var ssPass = new SecureString();

            foreach (var c in password)
            {
                ssPass.AppendChar(c);
            }
            ssPass.MakeReadOnly();

            return new SqlConnection(scsb.ToString(), new SqlCredential(user, ssPass));
        }

        private void bDbSave_Click(object sender, EventArgs e)
        {
            var _s = Properties.Settings.Default;

            _s.dbHost = tbDbHost.Text;
            _s.dbPassword = tbDbPassword.Text;
            _s.dbUsername = tbDbUsername.Text;
            _s.dbPort = ushort.Parse(tbDbPort.Text);
            _s.dbDefault = cbDbDefault.Text;

            _s.Save();
        }
    }
}
