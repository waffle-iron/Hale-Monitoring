using Dapper;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hale_DBTools
{
    public partial class FormDBTools : Form
    {
        public FormDBTools()
        {
            InitializeComponent();
        }

        private void FormDBTools_Load(object sender, EventArgs e)
        {
            LoadSettings();
            LoadSyntaxStyles();
        }

        #region Progress

        public void UpdateProgressPercent(string text = null)
        {
            tslProgressPercent.Text = text != null ? text :
                (((float)queryProgress.Value / queryProgress.Maximum)).ToString("P0");
        }

        public void ResetProgress()
        {
            queryProgress.Value = 0;
            queryProgress.Style = ProgressBarStyle.Blocks;
            UpdateProgressPercent("Idle");
            tabControl1.Enabled = true;
        }

        public void BeginBatch()
        {
            tabControl1.Enabled = false;

            tbOutput.Clear();
            tbQueries.Clear();
        }

        #endregion

        #region Control bindings

        private void bEditData_Click(object sender, EventArgs e)
        {
            var tableName = ((Control)sender).Tag as string;
            var fed = new FormEditData(tableName, GetConnection());
            fed.ShowDialog();
        }

        private void bDbRunMultiSQL_Click(object sender, EventArgs e)
        {
            string[] scripts = ((string)((Control)sender).Tag).Split('+');
            dbRunMultiSQL(scripts);
        }

        private void bDbRunSingleSQL_Click(object sender, EventArgs e)
        {
            var scriptName = (string)((Control)sender).Tag;
            dbRunSingleSQL(scriptName);
        }

        private void bDbRunMultiSQLSet_Click(object sender, EventArgs e)
        {
            List<string> scripts = new List<string>();
            string[] sets = ((string)((Control)sender).Tag).Split('+');
            foreach(string set in sets)
            {
                scripts.AddRange(SQLSets[set]);
            }
            dbRunMultiSQL(scripts.ToArray());
        }

        #endregion

        private void ConnectionContext_ServerMessage(object sender, ServerMessageEventArgs e)
        {
            Invoke(new Action(() => // run on GUI thread
            {
                if (e.Error.Class == 0)
                {
                    tbOutput.AppendText(e.Error.Message + Environment.NewLine);
                }
                else
                {
                    tbOutput.AppendText("ERROR: " + e.Error.Message + Environment.NewLine);
                }

            }));
            
        }



    }
}
