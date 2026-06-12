using DeploymentTool.Base;
using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIx = DeploymentTool.UI.UI;

namespace DeploymentTool.Services
{
    public class RollbackService : Loggable
    {
        private Settings _settings;
        private ZipService _zipService;
        private FileService _fileService;
        private static int _uxDelay = 500;

        #region Constructor
        public RollbackService(Settings settings, ILogger logger) : base(logger)
        {
            _settings = settings;
            _zipService = new(_logger);
            _fileService = new(_logger);
        }
        #endregion

        public async Task CreateBackup()
        {
            Log("Deleting old backup...");

            if (File.Exists(GetBackupPath()))
                File.Delete(GetBackupPath());

            Log("Old backup deleted");
            Log("Preparing to create backup...");

            await PrepareTargetForBackup();
            await _zipService.ZipFolderAsync(_settings.Target.Path, GetBackupPath());
            await RestoreTargetFromBackup();

            Log("Backup created");
        }

        public async Task Rollback()
        {
            if (!File.Exists(GetBackupPath()))
                throw new Exception($"No backup found for {_settings.Pipeline.Name} pipeline to restore!");

            Log("Preparing to rollback...");

            await CleanTargetForRollback();
            await _zipService.UnzipFolderAsync(GetBackupPath(), _settings.Target.Path);

            Log("Rollback completed");
        }

        protected override void Log(string message)
        {
            UIx.Clear(message, _uxDelay);
            _logger.Log("Rollback Service", message);
        }

        #region Private Methods
        private string GetZipName()
            => $"{_settings.Pipeline.Name}.zip";

        private string GetBackupPath()
            => Path.Combine(_settings.Pipeline.Rollback.BackupPath, GetZipName());

        private async Task PrepareTargetForBackup()
        {
            if (_settings.Target.FilesToKeep == null || !_settings.Target.FilesToKeep.Any())
                return;

            string tempFolder = Path.Combine(
                _settings.Pipeline.Rollback.BackupPath,
                $"{_settings.Pipeline.Name}_temp"
            );

            if (Directory.Exists(tempFolder))
                Directory.Delete(tempFolder, true);
            Directory.CreateDirectory(tempFolder);

            await _fileService.MoveFilesWithProgress(
                _settings.Target.Path,
                _settings.Target.FilesToKeep,
                tempFolder,
                "Moving keep-files to temp"
            );
        }

        private async Task RestoreTargetFromBackup()
        {
            string tempFolder = Path.Combine(
                _settings.Pipeline.Rollback.BackupPath,
                $"{_settings.Pipeline.Name}_temp"
            );

            if (!Directory.Exists(tempFolder))
                return;

            var tempFiles = Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories)
                                     .Select(f => Path.GetRelativePath(tempFolder, f))
                                     .ToArray();

            await _fileService.MoveFilesWithProgress(
                tempFolder,
                tempFiles,
                _settings.Target.Path,
                "Restoring keep-files to target"
            );

            Directory.Delete(tempFolder, true);
        }

        private async Task CleanTargetForRollback()
            => await _fileService.DeleteFilesExcept(_settings.Target.Path, _settings.Target.FilesToKeep ?? new string[0], "Cleaning target directory for rollback");
        #endregion
    }
}
