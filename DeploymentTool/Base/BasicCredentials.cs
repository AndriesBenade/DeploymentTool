using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Base
{
    public abstract class BasicCredentials
    {
        public abstract string Username { get; set; }
        public abstract string Password { get; set; }
    }
}
