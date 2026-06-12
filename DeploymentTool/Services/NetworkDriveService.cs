using DeploymentTool.Models;
using DeploymentTool.Models.SettingsModules;
using DeploymentTool.UI;
using System;
using System.Diagnostics;
using UIx = DeploymentTool.UI.UI;

namespace DeploymentTool.Services
{
    public class NetworkDriveService
    {
        /// <summary>
        /// Maps a network drive to the specified path. Uses credentials if provided, otherwise defaults to current user.
        /// </summary>
        public void MapDrive(string path, NetworkDriveCredentials creds = null)
        {
            if (creds != null && creds.HasCredentials())
            {
                UIx.LogStatus($"Mapping network drive to {path} using credentials {creds.Username}/******");
                ExecuteCommand($"net use \"{path}\" /user:{creds.Username} {creds.Password} /persistent:no");
            }
            else
            {
                UIx.LogStatus($"Mapping network drive to {path} (no credentials)");
                ExecuteCommand($"net use \"{path}\" /persistent:no");
            }
        }

        /// <summary>
        /// Disconnects the network drive.
        /// </summary>
        public void UnmapDrive(string path)
        {
            UIx.LogStatus($"Disconnecting network drive {path}");
            ExecuteCommand($"net use \"{path}\" /delete /y");
        }

        #region Private Methods
        private void ExecuteCommand(string cmd)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo("cmd.exe", $"/c {cmd}")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };
            proc.Start();
            proc.WaitForExit();
        }
        #endregion
    }
}
