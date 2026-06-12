using DeploymentTool.Base;
using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using DeploymentTool.Services;
using DeploymentTool.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIx = DeploymentTool.UI.UI;

namespace DeploymentTool.Deployments
{
    public class LocalDeployment : Deployment
    {
        private RollbackService _rollbackService;


        public LocalDeployment(Settings settings, string releaseName, ILogger logger) : base(settings, releaseName, logger)
        {
            _rollbackService = new(settings, logger);
        }

        public override async Task<bool> Deploy()
        {
            try
            {
                if (_settings.Pipeline.Rollback.Enabled)
                {
                    Log("Preparing Rollback service...");

                    await _rollbackService.CreateBackup();
                }
                else
                    Log("Rollback service is disabled.");

                Log("Initiating deployment...");

                bool success = false;
                if (_settings.Source.Zip)
                    success = await ZippedDeployment();
                else
                    success = await UnzippedDeployment();

                if (success)
                    Log("Deployment completed.");
                else
                {
                    Log("Deployment completed with errors.");
                    UIx.Clear("Deployment completed with errors!\nTo see what went wrong, refer to the logs.\n\nPress any key to exit...");
                    UIx.ReadKey();
                }

                return true;
            }
            catch (Exception ex)
            {
                if (_settings.Pipeline.Rollback.Enabled)
                {
                    UIx.Clear("Something went wrong!", 500);
                    UIx.Clear("Preparing to rollback...", 500);

                    await _rollbackService.Rollback();

                    ErrorService.HandleError(ex, "Deployment failed and has been rolled back!", _logger);
                }
                else
                    ErrorService.HandleError(ex, "An error occured while deploying! Rollback is disabled thus no backup was created and nothing was rolled back.", _logger);

                return false;
            }
        }

        protected override void Log(string message)
            => _logger.Log("Local Deployment", message);

        #region Deployment Methods
        private async Task<bool> ZippedDeployment()
        {
            try
            {
                StopAppPool();

                await CleanSource();

                DeleteFile(SourceZip);

                await ZipSource();
                
                if (_settings.Target.CleanTarget)
                    await CleanTarget();

                DeleteFile(TargetZip);

                await _fileService.CopyFileWithProgress(Target, Target);

                if (_settings.Deployment.Unzip)
                    await UnzipTarget();

                RestartAppPool();

                return true;
            }
            catch (Exception ex)
            {
                ErrorService.HandleError(ex, "An error occurred during zipped deployment!", _logger);
            }

            return false;
        }

        private async Task<bool> UnzippedDeployment()
        {
            try
            {
                StopAppPool();

                await CleanSource();
                
                if (_settings.Target.CleanTarget)
                    await CleanTarget();

                await _fileService.CopyFolderWithProgress(Source, Target);

                RestartAppPool();

                return true;
            }
            catch (Exception ex)
            {
                ErrorService.HandleError(ex, "An error occurred during unzipped deployment!", _logger);
            }

            return false;
        }
        #endregion
    }
}
