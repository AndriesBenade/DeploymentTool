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

namespace DeploymentTool.Base
{
    public abstract class Deployment : Loggable, IDeployment
    {
        protected string _releaseName;
        protected Settings _settings;
        protected ZipService _zipService;
        protected IISService _iisService;
        protected FileService _fileService;
        protected NetworkDriveService _networkDriveService;

        protected string Source => _settings.Source.Path;
        protected string Target => _settings.Target.Path;
        protected string SourceZip => Path.Combine(Source, $"{_releaseName}.zip");
        protected string TargetZip => Path.Combine(Target, $"{_releaseName}.zip");

        #region Constructor
        public Deployment(Settings settings, string releaseName, ILogger logger) : base(logger)
        {
            _settings = settings;
            _releaseName = releaseName;

            _fileService = new FileService(logger);
            _zipService = new ZipService(logger);
            _iisService = new IISService(_settings.Deployment.IIS.Host, logger, _settings.Credentials?.IIS);
            _networkDriveService = new NetworkDriveService();
        }
        #endregion

        public abstract Task<bool> Deploy();

        #region Protected Methods
        protected async Task CleanSource()
        {
            if (_settings.Source.FilesToDelete?.Any() == true)
                await _fileService.DeleteFilesWithProgress(Source, _settings.Source.FilesToDelete, "Deleting source files");
        }

        protected async Task CleanTarget()
        {
            if (_settings.Target.FilesToKeep?.Any() == true)
                await _fileService.DeleteFilesExcept(Target, _settings.Target.FilesToKeep, "Cleaning target directory");
        }

        protected async Task ZipSource()
            => await _zipService.ZipFolderAsync(Source, SourceZip);

        protected async Task UnzipTarget()
        {
            await _zipService.UnzipFolderAsync(TargetZip, Target);
            DeleteFile(TargetZip);
        }

        protected void StopAppPool()
        {
            if (_settings.Deployment.IIS?.Enabled == true)
                _iisService.StopAppPool(_settings.Deployment.IIS.AppPool);
        }

        protected void StartAppPool()
        {
            if (_settings.Deployment.IIS?.Enabled == true)
                _iisService.StartAppPool(_settings.Deployment.IIS.AppPool);
        }

        protected void RecycleAppPool()
        {
            if (_settings.Deployment.IIS?.Enabled == true)
                _iisService.RecycleAppPool(_settings.Deployment.IIS.AppPool);
        }

        protected void RestartAppPool()
        {
            StartAppPool();
            RecycleAppPool();
        }

        protected void DeleteFile(string file)
        {
            if (File.Exists(file)) File.Delete(file);
        }
        #endregion
    }
}
