using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using DeploymentTool.Services.Deployers;
using DeploymentTool.UI.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.UI.Menus
{
    public class MainMenu : Menu
    {
        #region Constructor
        public MainMenu(Settings settings, ILogger logger) : base(settings, "Main Menu", new List<MenuOption>(), 0, logger)
        {
            Options.Add(new MenuOption(0, "Quick Deploy", () => QuickDeploy()));
            Options.Add(new MenuOption(1, "Environments", () => Environments()));
            Options.Add(new MenuOption(2, "Pipelines", () => Pipelines()));
            Options.Add(new MenuOption(3, "Settings", () => Settings()));
            Options.Add(new MenuOption(4, "Exit", () => Exit(), ConsoleColor.Red));
        }
        #endregion

        #region Event Handlers
        public void QuickDeploy()
            => new DefaultService(_logger).Deploy("appsettings");

        public void Environments()
            => new EnvironmentMenu(_settings, _logger).Render();

        public void Pipelines()
            => new PipelineMenu(_settings, _logger).Render();

        public void Settings()
            => new SettingsMenu(_settings, _logger).Render();
        #endregion
    }
}
