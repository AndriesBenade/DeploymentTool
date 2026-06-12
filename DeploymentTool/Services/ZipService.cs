using UIx = DeploymentTool.UI.UI;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using DeploymentTool.Base;
using DeploymentTool.Interfaces;

namespace DeploymentTool.Services
{
    public class ZipService : Loggable
    {
        #region Constructor
        public ZipService(ILogger logger) : base(logger)
        { }
        #endregion

        /// <summary>
        /// Zips a file.
        /// </summary>
        public async Task ZipFolderAsync(string folder, string zipPath)
        {
            Log($"Zipping folder ({folder}) to ({zipPath})...");

            var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
            int total = files.Length;

            using var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create);

            for (int i = 0; i < total; i++)
            {
                string file = files[i];
                string entryName = Path.GetRelativePath(folder, file);
                zip.CreateEntryFromFile(file, entryName);

                int percent = (int)((i + 1) * 100.0 / total);
                UIx.RenderProgress(percent, $"Zipping {entryName}...");
                await Task.Delay(0);
            }

            Log("Zipped.");
        }

        /// <summary>
        /// Unzips a file.
        /// </summary>
        public async Task UnzipFolderAsync(string zipPath, string extractPath)
        {
            Log($"Unzipping zip ({zipPath}) to ({extractPath})...");

            using var zip = ZipFile.OpenRead(zipPath);
            int total = zip.Entries.Count;

            for (int i = 0; i < total; i++)
            {
                var entry = zip.Entries[i];
                string destFile = Path.Combine(extractPath, entry.FullName);
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                entry.ExtractToFile(destFile, true);

                int percent = (int)((i + 1) * 100.0 / total);
                UIx.RenderProgress(percent, $"Unzipping {entry.FullName}...");
                await Task.Delay(0);
            }

            Log("Unzipped.");
        }

        protected override void Log(string message)
            => _logger.Log("Zip Service", message);
    }
}
