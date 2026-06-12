using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Base
{
    public abstract class Mode : IMode
    {
        protected Settings _settings;

        #region Constructor
        public Mode(Settings settings)
            => _settings = settings;
        #endregion

        public abstract void Run();

        #region Protected Methods
        protected void RenderOptions()
        {

        }
        #endregion
    }
}
