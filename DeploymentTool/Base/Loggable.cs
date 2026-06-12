using DeploymentTool.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Base
{
    public abstract class Loggable
    {
        protected ILogger _logger;

        #region Constructor
        public Loggable(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        protected abstract void Log(string message);
    }
}
