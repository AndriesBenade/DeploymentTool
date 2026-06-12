using DeploymentTool.Base;
using DeploymentTool.Helpers;
using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using DeploymentTool.Models.SettingsModules;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIx = DeploymentTool.UI.UI;
using ILogger = DeploymentTool.Interfaces.ILogger;

namespace DeploymentTool.Services.Deployers
{
    public class EnvironmentService : Deployer
    {
        public static string Path => "environments/";

        #region Constructor
        public EnvironmentService(ILogger logger) : base(logger, Path, DeploymentType.Environment)
        { }
        #endregion
    }
}
