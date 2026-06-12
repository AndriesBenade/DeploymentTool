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
    public class NetworkDriveDeployment : Deployment
    {
        public NetworkDriveDeployment(Settings settings, string releaseName, ILogger logger) : base(settings, releaseName, logger)
        { }

        public override async Task<bool> Deploy()
        {
            throw new Exception("Network drive deployment has been deprecated!");

            #region Code
            //_networkDriveService.MapDrive(Target, _settings.Credentials?.NetworkDrive);

            //StopAppPool();

            //await CleanSource();

            //DeleteFile(SourceZip);

            //await ZipSource();
            //await CleanTarget();

            //DeleteFile(TargetZip);

            //await _fileService.CopyFileWithProgress(SourceZip, TargetZip);

            //if (_settings.Deployment.Unzip)
            //    await UnzipTarget();

            //RestartAppPool();

            //_networkDriveService.UnmapDrive(Target);
            #endregion
        }

        protected override void Log(string message)
            => _logger.Log("Network Drive Deployment", message);
    }
}
