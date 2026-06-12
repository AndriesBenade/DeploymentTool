using DeploymentTool.Base;
using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using DeploymentTool.Models.SettingsModules;
using Microsoft.Web.Administration;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using Environment = System.Environment;
using UIx = DeploymentTool.UI.UI;

namespace DeploymentTool.Services
{
    public class IISService : Loggable
    {
        private readonly string _serverName;
        private readonly IISCredentials _credentials;
        private const int _uxDelay = 500;

        #region Constructors
        public IISService(string serverName, ILogger logger, IISCredentials credentials = null) : base(logger)
        {
            _serverName = serverName;
            _credentials = credentials;
        }
        #endregion

        /// <summary>
        /// Stops an app pool.
        /// </summary>
        public void StopAppPool(string appPoolName)
        {
            if (IsLocalhost())
            {
                Log($"Stopping local app pool ({appPoolName})...");

                using var serverManager = new ServerManager();
                var pool = serverManager.ApplicationPools[appPoolName];
                if (pool != null && pool.State != ObjectState.Stopped)
                {
                    pool.Stop();

                    Log($"App pool ({appPoolName}) stopped.");
                }
                else
                    Log($"App pool ({appPoolName}) is already stopped or does not exist.");
            }
            else
            {
                ExecuteRemotePowerShell($"Stop-WebAppPool -Name '{appPoolName}'");
            }
        }

        /// <summary>
        /// Starts an app pool.
        /// </summary>
        public void StartAppPool(string appPoolName)
        {
            if (IsLocalhost())
            {
                Log($"Starting local app pool ({appPoolName})...");

                using var serverManager = new ServerManager();
                var pool = serverManager.ApplicationPools[appPoolName];
                if (pool != null && pool.State != ObjectState.Started)
                {
                    pool.Start();
                    
                    Log($"App pool ({appPoolName}) started.");
                }
                else
                    Log($"App pool ({appPoolName}) is already started or does not exist.");
            }
            else
                ExecuteRemotePowerShell($"Start-WebAppPool -Name '{appPoolName}'");
        }

        /// <summary>
        /// Recycles an app pool.
        /// </summary>
        public void RecycleAppPool(string appPoolName)
        {
            if (IsLocalhost())
            {
                Log($"Recycling local app pool ({appPoolName})...");

                using var serverManager = new ServerManager();
                var pool = serverManager.ApplicationPools[appPoolName];
                if (pool != null)
                {
                    pool.Recycle();

                    Log($"App pool ({appPoolName}) recycled.");
                }
                else
                    Log($"App pool ({appPoolName}) does not exist.");
            }
            else
                ExecuteRemotePowerShell($"Restart-WebAppPool -Name '{appPoolName}'");
        }

        /// <summary>
        /// Checks if the specified application pool exists.
        /// </summary>
        public bool AppPoolExists(string appPoolName)
        {
            if (IsLocalhost())
            {
                using var serverManager = new ServerManager();
                return serverManager.ApplicationPools[appPoolName] != null;
            }
            else
            {
                var command = $"Get-WebAppPoolState -Name '{appPoolName}'";
                var connectionInfo = new WSManConnectionInfo(
                    new Uri($"http://{_serverName}:5985/wsman"),
                    "http://schemas.microsoft.com/powershell/Microsoft.PowerShell",
                    _credentials != null
                        ? new PSCredential(_credentials.Username, ConvertToSecureString(_credentials.Password))
                        : null
                );
                connectionInfo.AuthenticationMechanism = _credentials != null
                    ? AuthenticationMechanism.Default
                    : AuthenticationMechanism.Negotiate;

                using var runspace = RunspaceFactory.CreateRunspace(connectionInfo);
                runspace.Open();

                using var pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(command);

                try
                {
                    var results = pipeline.Invoke();
                    return results.Count > 0;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    runspace.Close();
                }
            }
        }

        protected override void Log(string message)
        {
            _logger.Log("IIS Service", message);
            UIx.Clear(message, _uxDelay);
        }

        #region Helpers
        private bool IsLocalhost()
        {
            string lower = _serverName.ToLower();
            return lower == "localhost" || lower == "127.0.0.1" || lower == Environment.MachineName.ToLower();
        }

        private void ExecuteRemotePowerShell(string command)
        {
            Log($"Executing remote command on ({_serverName}): {command}...");

            var connectionInfo = new WSManConnectionInfo(
                new Uri($"http://{_serverName}:5985/wsman"),
                "http://schemas.microsoft.com/powershell/Microsoft.PowerShell",
                _credentials != null
                    ? new System.Management.Automation.PSCredential(
                        _credentials.Username,
                        ConvertToSecureString(_credentials.Password))
                    : null
            );
            connectionInfo.AuthenticationMechanism = _credentials != null
                ? AuthenticationMechanism.Default
                : AuthenticationMechanism.Negotiate;

            using var runspace = RunspaceFactory.CreateRunspace(connectionInfo);
            runspace.Open();

            using var pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(command);
            var results = pipeline.Invoke();

            foreach (var r in results)
                Log(r.ToString());

            runspace.Close();
        }

        private System.Security.SecureString ConvertToSecureString(string password)
        {
            if (string.IsNullOrEmpty(password))
                return null;

            var secure = new System.Security.SecureString();
            foreach (char c in password)
                secure.AppendChar(c);

            secure.MakeReadOnly();
            return secure;
        }
        #endregion
    }
}
