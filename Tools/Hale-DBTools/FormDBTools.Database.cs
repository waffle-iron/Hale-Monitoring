using FastColoredTextBoxNS;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hale_DBTools
{
    public partial class FormDBTools
    {
        TextStyle headerStyle = new TextStyle(Brushes.Blue, Brushes.Transparent, FontStyle.Bold);

        private void LoadSyntaxStyles()
        {
            tbOutput.AddStyle(headerStyle);
        }

        private async void dbRunSingleSQL(string scriptName)
        {
            SqlConnection con;
            var startTime = DateTime.Now;

            BeginBatch();

            if (scriptName[0] == '_') // Scripts beginning with _ means do connect to default DB
            {
                scriptName = scriptName.Substring(1);
                con = GetConnection(false);
            }
            else
            {
                con = GetConnection(true);
            }

            var sqlFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),
                "SQL", scriptName + ".sql");
            if (File.Exists(sqlFile))
            {
                using (var sr = new StreamReader(sqlFile))
                {
                    var sql = sr.ReadToEnd();
                    tbQueries.Clear();
                    tbQueries.AppendText("-- ### " + scriptName + Environment.NewLine + sql + Environment.NewLine);
                    tbQueries.GoEnd();


                    queryProgress.Style = ProgressBarStyle.Marquee;
                    UpdateProgressPercent("Running...");

                    var srvCon = new ServerConnection(con);
                    Server server = new Server(srvCon);
                    server.ConnectionContext.ServerMessage += ConnectionContext_ServerMessage;

                    try
                    {
                        tbOutput.AppendText("### Beginning execution script \"" + scriptName + "\"..." + Environment.NewLine, headerStyle);
                        tbOutput.GoEnd();
                        var x = await ExecuteNonQueryAsync(server, sql);
                        if (x.InnerException != null)
                        {
                            throw x.InnerException;
                        }
                        tbOutput.AppendText(String.Format("### Script \"{0}\" completed in {1} seconds with no errors.",
                            scriptName, (DateTime.Now - startTime).TotalSeconds.ToString("F2")) + Environment.NewLine, headerStyle);
                        tbOutput.GoEnd();
                    }
                    catch (Exception x)
                    {
                        tbOutput.AppendText(String.Format("### Script \"{0}\" stopped after {1} seconds with the following errors:{2}{3}{2}",
                            scriptName, (DateTime.Now - startTime).TotalSeconds.ToString("F2"), Environment.NewLine, x.Message), headerStyle);
                        tbOutput.GoEnd();

                        if (MessageBox.Show(String.Format("Error running SQL script \"{0}\":\n{1}\n\n",
                            scriptName, x.Message
                            ), "Error running SQL script", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                        {
                            ResetProgress();

                            return;
                        }
                    }

                }
            }
            else
            {
                MessageBox.Show("Cannot find script \"" + scriptName + "\"", "Error executing script", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ResetProgress();
        }

        private async void dbRunMultiSQL(string[] scripts)
        {

            var startTime = DateTime.Now;
            var lastProgress = startTime;

            BeginBatch();

            queryProgress.Maximum = scripts.Length * 2;
            queryProgress.Value = 0;
            UpdateProgressPercent();


            for (int i = 0; i < scripts.Length; ++i)
            {
                var scriptName = scripts[i];
                SqlConnection con;

                if (scriptName[0] == '_')
                {
                    scriptName = scriptName.Substring(1);
                    con = GetConnection(false);
                }
                else
                {
                    con = GetConnection(true);
                }

                var srvCon = new ServerConnection(con);
                Server server = new Server(srvCon);
                server.ConnectionContext.ServerMessage += ConnectionContext_ServerMessage;

                var sqlFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),
                    "SQL", scriptName + ".sql");
                if (File.Exists(sqlFile))
                {
                    using (var sr = new StreamReader(sqlFile))
                    {
                        var sql = sr.ReadToEnd();

                        tbQueries.AppendText("-- ### " + scriptName + Environment.NewLine + sql + Environment.NewLine + Environment.NewLine);
                        tbQueries.GoEnd();

                        queryProgress.Value = (i * 2) + 1;
                        UpdateProgressPercent();


                        try
                        {
                            tbOutput.AppendText("### Beginning execution script \"" + scriptName + "\"..." + Environment.NewLine);
                            tbOutput.GoEnd();
                            var x = await ExecuteNonQueryAsync(server, sql);
                            if (x.InnerException != null)
                            {
                                throw x.InnerException;
                            }
                            tbOutput.AppendText(String.Format("### Script \"{0}\" completed in {1} seconds with no errors.{2}{2}",
                                scriptName, (DateTime.Now - lastProgress).TotalSeconds.ToString("F2"), Environment.NewLine));
                            tbOutput.GoEnd();
                            lastProgress = DateTime.Now;
                        }
                        catch (Exception x)
                        {
                            tbOutput.AppendText(String.Format("### Script \"{0}\" stopped after {1} seconds with the following errors:{2}{3}{2}",
                                scriptName, (DateTime.Now - lastProgress).TotalSeconds.ToString("F2"), Environment.NewLine, x.Message));
                            tbOutput.GoEnd();
                            lastProgress = DateTime.Now;

                            if (MessageBox.Show(String.Format("Error running SQL script \"{0}\":\n{1}\n\nDo you want to continue running the rest of the scripts?",
                                scriptName, x.Message
                                ), "Error running SQL script", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            {
                                ResetProgress();
                                return;
                            }
                        }

                        queryProgress.Value = (i * 2) + 2;
                        UpdateProgressPercent();


                    }
                }
            }

            tbOutput.AppendText(Environment.NewLine + "Done! Total running time: " +
                (DateTime.Now - startTime).TotalSeconds.ToString("F2") + " seconds.");
            tbOutput.GoEnd();

            ResetProgress();
        }


        private Task<Exception> ExecuteNonQueryAsync(Server server, string sql)
        {
            return Task.Run(() =>
            {
                try
                {
                    server.ConnectionContext.ExecuteNonQuery(sql);
                    return new Exception();
                }
                catch (Exception x)
                {
                    return x;
                }
            });
        }
    }
}
