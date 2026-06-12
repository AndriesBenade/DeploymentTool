using DeploymentTool.Deployments;
using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using DeploymentTool.Models.SettingsModules;
using DeploymentTool.UI;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using UIx = DeploymentTool.UI.UI;

namespace DeploymentTool.Services
{
    public class DeploymentService
    {
        private readonly Settings _settings;
        private readonly string _releaseName;
        private readonly GitService _gitService;
        private readonly VisualStudioService _vsService;
        private readonly ILogger _logger;

        #region Constructor
        public DeploymentService(Settings settings, string releaseName)
        {
            _settings = settings;
            _releaseName = releaseName;

            _logger = GetLogger();
            _gitService = new GitService(settings, _logger);
            _vsService = new VisualStudioService(settings, _logger);
        }
        #endregion

        public async Task<bool> Deploy()
        {
            bool outcome = false;

            if (!await Git() && _settings.Git.FailOnError)
            {
                End();
                return outcome;
            }

            if (!await Rebuild() && !_settings.Publish.IgnoreRebuildErrors)
            {
                End();
                return outcome;
            }

            if (!await Publish())
            {
                End();
                return outcome;
            }

            try
            {
                switch (_settings.Deployment.Mode)
                {
                    case DeployMode.Local:          outcome = await new LocalDeployment(_settings, _releaseName, _logger).Deploy(); break;
                    case DeployMode.NetworkDrive:   throw new Exception("Network drive deployment has been deprecated!");
                }

                UIx.Clear();
                UIx.LogStatus("Deployment finished.");
            }
            catch (Exception ex)
            {
                ErrorService.HandleError(ex, "An error occurred during deployment!", _logger);
            }

            End();
            return outcome;
        }

        #region Private Methods
        private void End()
        {
            ConsoleService.Beep();
        }

        private async Task<bool> Git()
        {
            try
            {
                _logger.Log("Deployment Service", "Starting Git service...");
                _gitService.Execute();
            }
            catch (Exception ex)
            {
                if (_settings.Git.FailOnError)
                {
                    ErrorService.HandleError(ex, "An error occurred while doing git operations!", _logger);
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> Rebuild()
        {
            try
            {
                await _vsService.RebuildSolution();
            }
            catch (Exception ex)
            {
                if (!_settings.Publish.IgnoreRebuildErrors)
                {
                    ErrorService.HandleError(ex, "An error occurred while rebuilding solution!", _logger);
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> Publish()
        {
            try
            {
                await _vsService.PublishProject();
                return true;
            }
            catch (Exception ex)
            {
                ErrorService.HandleError(ex, "An error occurred while publishing project!", _logger);
            }

            return false;
        }

        private ILogger GetLogger()
        {
            return _settings.Deployment.Logging.Mode switch
            {
                LoggerMode.File => new Logging.FileLogger(_settings),
                LoggerMode.Sql => new Logging.SqlLogger(_settings),
                _ => throw new Exception("Unrecognized logger mode!")
            };
        }
        #endregion
    }
}
