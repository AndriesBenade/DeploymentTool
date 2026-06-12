using DeploymentTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = DeploymentTool.Models.Environment;

namespace DeploymentTool.Interfaces
{
    public interface IDeployer
    {
        /// <summary>
        /// Executes/runs the deployment.
        /// </summary>
        /// <param name="name"></param>
        void Deploy(string name);

        /// <summary>
        /// Executes/runs the deployment.
        /// </summary>
        void Deploy(Environment env);
    }
}
