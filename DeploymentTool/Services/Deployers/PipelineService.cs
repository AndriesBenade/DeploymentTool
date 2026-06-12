using DeploymentTool.Base;
using DeploymentTool.Helpers;
using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using DeploymentTool.Models.SettingsModules;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIx = DeploymentTool.UI.UI;

namespace DeploymentTool.Services.Deployers
{
    public class PipelineService : Deployer
    {
        public static string Path = "pipelines/";

        #region Constructor
        public PipelineService(ILogger logger) : base(logger)
        { }
        #endregion
    }
}
