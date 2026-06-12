using DeploymentTool.Helpers;
using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Base
{
    public abstract class Logger : ILogger
    {
        protected Settings _settings;

        #region Protected Properties
        protected bool LoggingEnabled => _settings.Deployment.Logging.Enabled;
        #endregion

        #region Constructor
        public Logger(Settings settings)
        {
            new SqlLogTable(settings.Deployment.Logging.Sql.ConnectionString).Persist();
            _settings = settings;
        }
        #endregion

        public virtual void Log(string context, string message)
        {
            if (!LoggingEnabled) return;

            PersistLog(context, message);
        }

        //public abstract string[] RenderLogs();

        protected abstract void PersistLog(string context, string message);
    }
}
