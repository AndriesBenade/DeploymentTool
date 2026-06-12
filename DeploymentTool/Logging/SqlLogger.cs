using DeploymentTool.Base;
using DeploymentTool.Helpers;
using DeploymentTool.Models;
using DeploymentTool.Services;
using Markdig.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Logging
{
    public class SqlLogger : Logger
    {
        private string _connectionString => _settings.Deployment.Logging.Sql.ConnectionString;
        private SqlService _sql;

        #region Constructor
        public SqlLogger(Settings settings) : base(settings)
            => Initialize();
        #endregion

        protected override void PersistLog(string context, string message)
        {
            var logDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            _sql.CommitCommand($@"INSERT INTO DEPLOY_Log (Pipeline, LogDate, Context, Message) VALUES ('{_settings.Pipeline.Name}', '{logDate}', '{context}', '{message}')");
        }

        #region Private Methods
        private void Initialize()
        {
            _sql = new SqlService(_connectionString);
        }
        #endregion
    }
}
