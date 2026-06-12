using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Models.SettingsModules
{
    public class PipelineSettings
    {
        public string Name { get; set; }
        public RollbackSettings Rollback { get; set; }
    }
}
