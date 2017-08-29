using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Globalization;

namespace Ukadc.Diagnostics.Tests
{
    static class DatabaseHelper
    {
        public const string SERVER_NAME = ".\\SQLEXPRESS";
 
        public static string BuildConnectionString()
        {
            return BuildConnectionString(null);
        }

        public static string BuildConnectionString(string databaseName)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            if (!string.IsNullOrEmpty(databaseName))
            {
                builder.InitialCatalog = databaseName;
            }
            builder.DataSource = DatabaseHelper.SERVER_NAME;
            builder.IntegratedSecurity = true;
            builder.UserInstance = false;
            return builder.ToString();
        }

        public static void CreateDatabaseIfNotExists(string name, string folder)
        {
            ServerConnection connection = new ServerConnection();
            connection.ConnectionString = DatabaseHelper.BuildConnectionString();

            Server server = new Server(connection);

            if (!server.Databases.Contains(name))
            {
                string sqlCommand = string.Format(CultureInfo.InvariantCulture,
                                             "CREATE DATABASE {0} ON (NAME = {0}, FILENAME = '{1}\\{0}.mdf') LOG ON (NAME = {0}_LOG, FILENAME = '{1}\\{0}_log.ldf');",
                                             name, folder);

                server.ConnectionContext.ExecuteNonQuery(sqlCommand);
            }
           
            connection.Disconnect();
        }

        public static void DropDatabase(string name)
        {
            ServerConnection connection = new ServerConnection();
            connection.ConnectionString = DatabaseHelper.BuildConnectionString();

            Server server = new Server(connection);
            if (server.Databases.Contains(name))
            {
                server.Databases[name].Drop();
            }
            connection.Disconnect();
        }
    }
}
