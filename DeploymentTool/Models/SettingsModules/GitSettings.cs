using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Models.SettingsModules
{
    public class GitSettings
    {
        public bool Enabled { get; set; }
        public bool FailOnError { get; set; }
        public string RepositoryPath { get; set; }
        public string Branch { get; set; }
        public bool AutoStash { get; set; }
        public bool AutoPull { get; set; }
    }
}
