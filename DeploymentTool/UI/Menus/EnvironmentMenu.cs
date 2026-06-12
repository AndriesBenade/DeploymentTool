using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using DeploymentTool.Services;
using DeploymentTool.Services.Deployers;
using DeploymentTool.UI.Interactive;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.UI.Menus
{
    public class EnvironmentMenu : Menu
    {
        private EnvironmentService _deployer;

        #region Constructor
        public EnvironmentMenu(Settings settings, ILogger logger) : base(settings, "Environments", new List<MenuOption>(), 0, logger)
        {
            _deployer = new(_logger);
            var envs = _deployer.GetFiles();
            
            int i = 0;
            foreach (var e in envs)
                Options.Add(new MenuOption(i++, e, () => ManageEnvironment(e)));

            Options.Add(new MenuOption(i++, "Back", () => Exit(), ConsoleColor.Red));
        }
        #endregion

        #region Event Handlers
        public void ManageEnvironment(string name)
            => new ManageEnvironmentMenu(_settings, name, _logger).Render();
        #endregion
    }
}
