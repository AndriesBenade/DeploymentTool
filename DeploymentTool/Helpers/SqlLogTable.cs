using DeploymentTool.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Helpers
{
    public class SqlLogTable
    {
        private string _connectionString;
        private SqlService _sql;

        #region Constructor
        public SqlLogTable(string connectionString)
        {
            _connectionString = connectionString;

            _sql = new SqlService(_connectionString);
        }
        #endregion

        public void Persist()
        {
            _sql.CommitCommand(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DEPLOY_Log' AND xtype='U')
                CREATE TABLE DEPLOY_Log
                (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    LogDate DATETIME NOT NULL,
                    Pipeline NVARCHAR(255) NOT NULL,
                    Context NVARCHAR(255) NOT NULL,
                    Message NVARCHAR(MAX) NOT NULL
                )");
        }
    }
}
