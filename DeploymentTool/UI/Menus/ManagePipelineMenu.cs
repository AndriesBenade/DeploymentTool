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
using System.Text;
using System.Threading.Tasks;
using UIx = DeploymentTool.UI.UI;

namespace DeploymentTool.UI.Menus
{
    public class ManagePipelineMenu : Menu
    {
        private PipelineService _deployer;

        #region Constructor
        public ManagePipelineMenu(Settings settings, string pipelineName, ILogger logger) : base(settings, $"Manage Pipeline {pipelineName}", new List<MenuOption>(), 0, logger)
        {
            _deployer = new(_logger);

            Options.Add(new MenuOption(0, "View Settings", () => ViewSettings(pipelineName)));
            Options.Add(new MenuOption(1, "Validate", () => Validate(pipelineName)));
            Options.Add(new MenuOption(2, "Rollback", () => Rollback(pipelineName), ConsoleColor.Yellow));
            Options.Add(new MenuOption(3, "Deploy", () => Deploy(pipelineName)));
            Options.Add(new MenuOption(4, "Back", () => Exit(), ConsoleColor.Red));
        }

        public ManagePipelineMenu(Settings settings, Settings pipeline, ILogger logger) : base(settings, $"Manage Pipeline {pipeline.Pipeline.Name}", new List<MenuOption>(), 0, logger)
        {
            _deployer = new(_logger);

            Options.Add(new MenuOption(0, "View Settings", () => ViewSettings(pipeline)));
            Options.Add(new MenuOption(1, "Validate", () => Validate(pipeline)));
            Options.Add(new MenuOption(2, "Back", () => Exit(), ConsoleColor.Red));
        }
        #endregion

        #region Event Handlers
        public void ViewSettings(string pipelineName)
            => SystemRenderer.RenderSettings(_deployer.Get(pipelineName));

        public void ViewSettings(Settings pipeline)
            => SystemRenderer.RenderSettings(pipeline);

        public void Validate(string pipelineName)
        {
            Settings pipeline = _deployer.Get(pipelineName);
            var validator = new ValidationService();
            validator.ValidatePipeline(pipeline);
        }

        public void Validate(Settings pipeline)
        {
            var validator = new ValidationService();
            validator.ValidatePipeline(pipeline);
        }

        public void Deploy(string pipelineName)
            => _deployer.Deploy(pipelineName);

        public void Rollback(string pipelineName)
        {
            bool confirmed = UIx.Confirmation(
                $"Confirm Rollback ({pipelineName})",
                new List<string>
                {
                    "Are you sure you want to start a rollback for this pipeline?",
                    "Once the process starts, there is no way to stop/cancel it."
                }
            );
            if (!confirmed) return;

            Settings pipeline = _deployer.Get(pipelineName);
            var rollback = new RollbackService(pipeline, _logger);
            rollback.Rollback().Wait();
        }
        #endregion
    }
}
