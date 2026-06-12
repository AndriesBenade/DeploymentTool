using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Models.SettingsModules
{
    public class RollbackSettings
    {
        public bool Enabled { get; set; }
        public string BackupPath { get; set; }
    }
}
