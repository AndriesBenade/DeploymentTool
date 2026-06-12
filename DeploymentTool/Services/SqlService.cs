using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Services
{
    public class SqlService
    {
        private bool _valid = false;
        private string _connectionString;

        #region Constructor
        public SqlService(string connectionString)
        {
            _connectionString = connectionString;
            Validate();
        }
        #endregion

        /// <summary>
        /// Commit a non query SQL command.
        /// </summary>
        public void CommitCommand(string sql)
        {
            try
            {
                if (!_valid) return;

                using (var c = GetConnection())
                {
                    c.Open();

                    using (var cmd = c.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorService.HandleError(ex, "SQL Command Execution Failed");
            }
        }

        /// <summary>
        /// Commit a query SQL command that returns results.
        /// </summary>
        public List<object[]> CommitQuery(string sql)
        {
            List<object[]> results = new List<object[]>();
            if (!_valid) return results;
            
            try
            {
                using (var c = GetConnection())
                {
                    c.Open();
                    using (var cmd = c.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                object[] row = new object[reader.FieldCount];
                                reader.GetValues(row);
                                results.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorService.HandleError(ex, "SQL Query Execution Failed");
            }

            return results;
        }

        /// <summary>
        /// Tests the SQL connection.
        /// </summary>
        public bool TestConnection(out Exception error)
        {
            try
            {
                GetConnection().Open();
                GetConnection().Close();

                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex;
                return false;
            }
        }

        #region Private Methods
        private void Validate()
        {
            try
            {
                GetConnection().Open();
                GetConnection().Close();

                _valid = true;
            }
            catch (Exception ex)
            {
                _valid = false;
                ErrorService.HandleError(ex, "SQL Connection Failed");
            }
        }

        private SqlConnection GetConnection()
            => new SqlConnection(_connectionString);
        #endregion
    }
}
