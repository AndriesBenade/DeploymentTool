using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Models.SettingsModules
{
    public class TargetSettings
    {
        public string Path { get; set; }
        public bool CleanTarget { get; set; }
        public string[] FilesToKeep { get; set; }

        public void Initialize()
        {
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
        }

        public string[] GetFilesInTarget(string searchPattern = "*.zip")
            => Directory.GetFiles(Path, searchPattern);
    }
}
