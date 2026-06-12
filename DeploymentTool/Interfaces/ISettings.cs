using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Interfaces
{
    public interface ISettings
    {
        /// <summary>
        /// Gets the credentials for the current mode.
        /// </summary>
        ICredentials? GetCredentials();
    }
}
