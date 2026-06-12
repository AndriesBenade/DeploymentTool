using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Models.SettingsModules
{
    public class IISSettings
    {
        public bool Enabled { get; set; }
        public string Host { get; set; }
        public string AppPool { get; set; }
    }
}
