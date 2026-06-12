using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using DeploymentTool.Services;
using DeploymentTool.Services.Deployers;
using DeploymentTool.UI.Interactive;
using Microsoft.Extensions.Hosting;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DeploymentTool.UI.Menus
{
    public class PipelineMenu : Menu
    {
        private PipelineService _deployer;

        #region Constructor
        public PipelineMenu(Settings settings, ILogger logger) : base(settings, "Pipelines", new List<MenuOption>(), 0, logger)
        {
            _deployer = new(_logger);
            var pipelines = _deployer.GetFiles();
            
            int i = 0;
            foreach (var p in pipelines)
                Options.Add(new MenuOption(i++, p, () => ManagePipeline(p)));

            Options.Add(new MenuOption(i++, "Back", () => Exit(), ConsoleColor.Red));
        }

        public PipelineMenu(Settings settings, string envName, ILogger logger) : base(settings, $"{Path.GetFileNameWithoutExtension(envName)} Pipelines", new List<MenuOption>(), 0, logger)
        {
            _deployer = new(_logger);
            var pipelines = SettingsService.GetEnvPipelines(envName);

            int i = 0;
            foreach (var p in pipelines)
                Options.Add(new MenuOption(i++, p.Pipeline.Name, () => ManagePipeline(p)));

            Options.Add(new MenuOption(i++, "Back", () => Exit(), ConsoleColor.Red));
        }
        #endregion

        #region Event Handlers
        public void ManagePipeline(string name)
            => new ManagePipelineMenu(_settings, name, _logger).Render();

        public void ManagePipeline(Settings pipeline)
            => new ManagePipelineMenu(_settings, pipeline, _logger).Render();
        #endregion
    }
}
