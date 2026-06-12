using UIx = DeploymentTool.UI.UI;
using System;
using System.IO;
using System.Threading.Tasks;
using DeploymentTool.Base;
using DeploymentTool.Interfaces;
using System.Security.AccessControl;
using System.Security.Principal;

namespace DeploymentTool.Services
{
    public class FileService : Loggable
    {
        #region Constructor
        public FileService(ILogger logger) : base(logger)
        { }
        #endregion

        /// <summary>
        /// Copies a file from source to dest.
        /// </summary>
        public async Task CopyFileWithProgress(string source, string dest)
        {
            Log("Starting file copy...");

            EnsureFolderPermissions(Path.GetDirectoryName(dest)!);

            using var sourceStream = File.OpenRead(source);
            using var destStream = File.Create(dest);

            long total = sourceStream.Length;
            byte[] buffer = new byte[81920];
            long copied = 0;

            int read;
            while ((read = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await destStream.WriteAsync(buffer, 0, read);
                copied += read;
                int percent = (int)(copied * 100 / total);
                UIx.RenderProgress(percent, $"Copying {Path.GetFileName(source)}");
            }

            Log("File copied.");
        }

        /// <summary>
        /// Copies an entire folder from source to target.
        /// </summary>
        public async Task CopyFolderWithProgress(string sourceFolder, string targetFolder)
        {
            Log("Starting folder copy...");

            if (!Directory.Exists(sourceFolder))
                throw new DirectoryNotFoundException($"Source folder not found: {sourceFolder}");

            EnsureFolderPermissions(targetFolder);

            var files = Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories);

            long totalBytes = 0;
            foreach (var f in files)
                totalBytes += new FileInfo(f).Length;

            long copiedBytes = 0;

            foreach (var file in files)
            {
                string relative = Path.GetRelativePath(sourceFolder, file);
                string dest = Path.Combine(targetFolder, relative);

                EnsureFolderPermissions(Path.GetDirectoryName(dest)!);

                using var sourceStream = File.OpenRead(file);
                using var destStream = File.Create(dest);

                byte[] buffer = new byte[81920];
                int read;

                while ((read = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await destStream.WriteAsync(buffer, 0, read);
                    copiedBytes += read;

                    int percent = totalBytes == 0 ? 100 : (int)(copiedBytes * 100 / totalBytes);
                    UIx.RenderProgress(percent, $"Copying {relative}");
                }
            }

            Log("Folder copied.");
        }

        /// <summary>
        /// Deletes the specified files in a folder.
        /// </summary>
        public async Task DeleteFilesWithProgress(string folder, string[] files, string status)
        {
            Log("Starting file deletion...");

            EnsureFolderPermissions(folder);

            int total = files.Length;
            for (int i = 0; i < total; i++)
            {
                string file = Path.Combine(folder, files[i]);
                if (File.Exists(file))
                    File.Delete(file);

                int progress = (int)((i + 1) * 100.0 / total);
                UIx.RenderProgress(progress, $"{status}: {files[i]} ({i + 1}/{total})");
                await Task.Delay(0);
            }

            Log("File deletion completed.");
        }

        /// <summary>
        /// Deletes all files in a folder except the specified files.
        /// </summary>
        public async Task DeleteFilesExcept(string folder, string[] filesToKeep, string status)
        {
            Log("Starting selective file deletion...");

            EnsureFolderPermissions(folder);

            var allFiles = Directory.GetFiles(folder, "*", SearchOption.TopDirectoryOnly);
            int total = allFiles.Length;

            for (int i = 0; i < total; i++)
            {
                string file = allFiles[i];
                string fileName = Path.GetFileName(file);

                if (!filesToKeep.Contains(fileName, StringComparer.OrdinalIgnoreCase))
                    File.Delete(file);

                int percent = (int)((i + 1) * 100.0 / total);
                UIx.RenderProgress(percent, $"{status}: {fileName} ({i + 1}/{total})");
                await Task.Delay(0);
            }

            var allDirs = Directory.GetDirectories(folder, "*", SearchOption.AllDirectories);
            foreach (var dir in allDirs)
            {
                if (!Directory.EnumerateFileSystemEntries(dir).Any())
                    Directory.Delete(dir, false);
            }

            Log("Selective file deletion completed.");
        }

        /// <summary>
        /// Moves specified files to a folder.
        /// </summary>
        public async Task MoveFilesWithProgress(string baseFolder, string[] filePaths, string destinationFolder, string status)
        {
            Log("Starting file movement...");

            if (filePaths == null || filePaths.Length == 0)
                return;

            EnsureFolderPermissions(destinationFolder);
            EnsureFolderPermissions(baseFolder);

            int total = filePaths.Length;

            for (int i = 0; i < total; i++)
            {
                string relativePath = filePaths[i];
                string sourcePath = Path.Combine(baseFolder, relativePath);
                string destPath = Path.Combine(destinationFolder, relativePath);

                string? destDir = Path.GetDirectoryName(destPath);
                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir!);

                if (File.Exists(sourcePath))
                    File.Move(sourcePath, destPath, true);

                int percent = (int)((i + 1) * 100.0 / total);
                UIx.RenderProgress(percent, $"{status}: {relativePath}");
                await Task.Delay(0);
            }

            Log("File movement completed.");
        }

        protected override void Log(string message)
            => _logger.Log("File Service", message);

        #region Private Methods
        private void EnsureFolderPermissions(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var dirInfo = new DirectoryInfo(folder);
            var adminSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
            var security = dirInfo.GetAccessControl();

            security.SetOwner(adminSid);

            security.AddAccessRule(
                new FileSystemAccessRule(
                    adminSid,
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow
                )
            );

            dirInfo.SetAccessControl(security);
        }
        #endregion
    }
}
