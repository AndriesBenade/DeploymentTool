using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Interfaces
{
    public interface ICredentials
    {
        /// <summary>
        /// Checks if the credentials have been provided.
        /// </summary>
        bool HasCredentials();
    }
}
