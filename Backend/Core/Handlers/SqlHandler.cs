using Hale_Core.Config;
using NLog;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security;
using System.Threading;


namespace Hale_Core.Handlers
{
    abstract internal class SqlHandler
    {
        private string connectionString;
        private string databaseName;

        private Logger log;
        private Configuration config;

        private readonly int connectionAttempts = 3;
        private readonly int connectionDelay = 3;

        internal SqlConnection connection;

        internal SqlHandler()
        {
            databaseName = "HaleDB";

            LoadLogger();
            LoadConfiguration();
            LoadConnectionString();

        }

        private void LoadLogger()
        {
            log = LogManager.GetCurrentClassLogger();
        }

        private void LoadConfiguration()
        {
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // Note: I have replaced the instanced variable with config.Database(), which has to be
            // a method since property extensions does not exist in C#. 
            // Perhaps this should be reverted? -NM 2016-01-17
        }


        private void LoadConnectionString()
        {
            try
            {
                SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
                connectionStringBuilder.DataSource = config.Database().Host;
                connectionStringBuilder.InitialCatalog = config.Database().Database;
                connectionStringBuilder.IntegratedSecurity = config.Database().UseIntegratedSecurity;

                connectionString = connectionStringBuilder.ToString();
            }
            catch
            {
                log.Error("Could not setup connection string for \"" + databaseName + "\". Check HaleCore.config");
            }

        }

        private SqlCredential LoadCredentials()
        {
            var ca = config.Database().Password.ToCharArray();
            SecureString pw = new SecureString();
            foreach (char t in ca)
            {
                pw.AppendChar(t);
            }
            pw.MakeReadOnly();
            return new SqlCredential(config.Database().User, pw);

        }

        internal void ConnectToDatabase()
        {
            connection = null;

            if (config.Database().UseIntegratedSecurity)
            {
                connection = new SqlConnection(connectionString);

            }
            else
            {
                SqlCredential credentials = LoadCredentials();
                connection = new SqlConnection(connectionString, credentials);
            }
            ConnectWithRetries();

        }

        private void ConnectWithRetries()
        {
            for (int i = 1; i < connectionAttempts; i++)
            {
                try
                {
                    connection.Open();
                    break;
                }
                catch (InvalidOperationException e)
                {
                    log.Error("Could not execute the requested operation:" + e.Message);
                }
                catch (SqlException e)
                {
                    if (i < connectionAttempts)
                    {
                        log.Warn(
                            "Connection attempt " + i + " of " + connectionAttempts
                            + " failed. Retrying in " + connectionDelay + " seconds."
                        );
                        Thread.Sleep(TimeSpan.FromSeconds(connectionDelay));
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
        }
    }
}
