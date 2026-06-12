using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Models.SettingsModules
{
    public class SourceSettings
    {
        public bool Zip { get; set; }
        public string Path { get; set; }
        public string[] FilesToDelete { get; set; }
    }
}
