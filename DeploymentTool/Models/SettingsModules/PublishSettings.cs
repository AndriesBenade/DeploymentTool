using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Models.SettingsModules
{
    public class PublishSettings
    {
        public bool Enabled { get; set; }
        public bool IgnoreRebuildErrors { get; set; }
        public string SolutionFile { get; set; }
        public string ProjectFile { get; set; }
    }
}
