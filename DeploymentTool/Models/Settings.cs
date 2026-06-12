using DeploymentTool.Base;
using DeploymentTool.Interfaces;
using DeploymentTool.Logging;
using DeploymentTool.Models.SettingsModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Models
{
    /// <summary>
    /// Default settings as well as normal pipeline.
    /// </summary>
    public class Settings : ISettings
    {
        public static string Version = "2.4";
        private string _filename;

        public PipelineSettings Pipeline { get; set; }
        public GitSettings Git { get; set; }
        public PublishSettings Publish { get; set; }
        public SourceSettings Source { get; set; }
        public TargetSettings Target { get; set; }
        public DeploymentSettings Deployment { get; set; }
        public Credentials Credentials { get; set; }
        public ExtraSettings Extra { get; set; }

        public void Initialize(string filename)
        {
            SetFilename(filename);
            Target.Initialize();
        }

        public ICredentials? GetCredentials()
            => Deployment.Mode switch
            {
                DeployMode.NetworkDrive => Credentials.NetworkDrive,
                _ => null,
            };

        public void SetFilename(string filename)
            => _filename = filename;

        public string GetFilename()
            => _filename;

        public ILogger GetLogger()
            => Deployment.Logging.Mode switch
            {
                LoggerMode.File => new FileLogger(this),
                LoggerMode.Sql => new SqlLogger(this),
                _ => throw new Exception("Unrecognized logging mode!"),
            };
    }
}
