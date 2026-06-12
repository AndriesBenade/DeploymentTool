using DeploymentTool.Base;
using DeploymentTool.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Models.SettingsModules
{
    public class Credentials
    {
        public IISCredentials IIS { get; set; }
        public NetworkDriveCredentials NetworkDrive { get; set; }
    }

    public class NetworkDriveCredentials : BasicCredentials, ICredentials
    {
        public override string Username { get; set; }
        public override string Password { get; set; }

        public bool HasCredentials()
            => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
    }

    public class IISCredentials : BasicCredentials, ICredentials
    {
        public override string Username { get; set; }
        public override string Password { get; set; }

        public bool HasCredentials()
            => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
    }
}
