using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Interfaces
{
    public interface IDeployment
    {
        Task<bool> Deploy();
    }
}
