using DeploymentTool.Helpers;
using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using DeploymentTool.Services;
using DeploymentTool.Services.Deployers;
using DeploymentTool.UI.Interactive;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Environment = DeploymentTool.Models.Environment;
using UIx = DeploymentTool.UI.UI;

namespace DeploymentTool.UI.Menus
{
    public class ManageEnvironmentMenu : Menu
    {
        private EnvironmentService _deployer;

        #region Constructor
        public ManageEnvironmentMenu(Settings settings, string envName, ILogger logger) : base(settings, $"Manage Environment {envName}", new List<MenuOption>(), 0, logger)
        {
            _deployer = new(_logger);

            Options.Add(new MenuOption(0, "Pipelines", () => ManagePipelines(envName)));
            Options.Add(new MenuOption(1, "Validate", () => Validate(envName)));
            Options.Add(new MenuOption(2, "Rollback", () => Rollback(envName), ConsoleColor.Yellow));
            Options.Add(new MenuOption(3, "Deploy", () => Deploy(envName)));
            Options.Add(new MenuOption(4, "Back", () => Exit(), ConsoleColor.Red));
        }
        #endregion

        #region Event Handlers
        public void ManagePipelines(string name)
            => new PipelineMenu(_settings, $"{name}.json", _logger).Render();

        public void Validate(string name)
        {
            Environment env = _deployer.Get(name);
            var validator = new ValidationService();
            validator.ValidateEnvironment(env);
        }

        public void Deploy(string name)
            => _deployer.Deploy(name);

        public void Rollback(string name)
        {
            bool confirmed = UIx.Confirmation(
                $"Confirm Rollback ({name})",
                new List<string>
                {
                    "Are you sure you want to start a rollback for this environment?",
                    "Once the process starts, there is no way to stop/cancel it."
                }
            );
            if (!confirmed) return;

            Environment env = _deployer.Get(name);
            foreach (var p in env.Pipelines)
            {
                var rollback = new RollbackService(p, _logger);
                rollback.Rollback().Wait();
            }
        }
        #endregion
    }
}
