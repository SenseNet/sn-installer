using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseNet.Installer
{
    internal class DatabaseManager
    {
        internal static async Task<bool> DatabaseExists(string dataSource, string databaseName)
        {
            // Do not fill the InitialCatalog property here, because we want to check
            // the existence of the db below, even if it does not exist yet.
            var csb = new SqlConnectionStringBuilder
            {
                DataSource = dataSource,
                IntegratedSecurity = true
            };

            using (var connection = new SqlConnection(csb.ToString()))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var cmd = new SqlCommand($"SELECT db_id('{databaseName}')", connection))
                    {
                        var result = await cmd.ExecuteScalarAsync();
                        if (result == null || result == DBNull.Value)
                            return false;
                        
                        return Convert.ToInt32(result) > 0;
                    }
                }
                catch (SqlException ex)
                {
                    Logger.WriteLine(ex.ToString());

                    throw new DataException("Error during database check.", ex);
                }
            }
        }
    }
}
