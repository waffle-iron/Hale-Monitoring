using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hale.DBTools
{
    public partial class FormEditData : Form
    {
        SqlConnection con;
        string tableName;

        public FormEditData(string tableName, SqlConnection con)
        {
            InitializeComponent();
            this.con = con;
            this.tableName = tableName;
        }

        private void FormEditData_Load(object sender, EventArgs e)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            switch(tableName)
            {
                case "Hosts":
                    adapter = new SqlDataAdapter("SELECT * FROM Nodes.Hosts", con);
                    break;
                case "Users":
                    adapter = new SqlDataAdapter("SELECT * FROM Security.Users", con);
                    break;
                default:
                    if (MessageBox.Show("Cannot edit data of type \"" + tableName + "\"", "Cannot edit data",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                    {
                        Close();
                    }
                    break;

            }

            Text += " [" + tableName + "]";

            var dataSet = new DataSet(tableName);

            adapter.Fill(dataSet, tableName);

            dataGridView1.DataSource = dataSet.Tables[tableName];
        }
    }
}
