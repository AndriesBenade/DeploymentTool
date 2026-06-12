using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Models.SettingsModules
{
    public class DeploymentSettings
    {
        public DeployMode Mode { get; set; }
        public bool Unzip { get; set; }
        public IISSettings IIS { get; set; }
        public LoggerSettings Logging { get; set; }
    }

    public enum DeployMode
    {
        Local = 0,
        NetworkDrive = 1
    }
}
