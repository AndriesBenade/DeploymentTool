using DeploymentTool.Models;
using DeploymentTool.Models.SettingsModules;
using DeploymentTool.Services.Deployers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Environment = DeploymentTool.Models.Environment;

namespace DeploymentTool.Services
{
    public class SettingsService
    {
        private readonly string _filename;

        #region Constructors
        public SettingsService(string filename)
        {
            _filename = filename;
        }
        #endregion

        #region Static Methods
        public static List<Settings> GetEnvPipelines(string filename)
        {
            var env = new SettingsService(Path.Combine(EnvironmentService.Path, filename)).LoadEnv();
            return env.Pipelines;
        }
        #endregion

        /// <summary>
        /// Loads settings from specified file.
        /// </summary>
        public Settings Load()
        {
            var settings = Load<Settings>(_filename);
            settings.Initialize(_filename);

            return settings;
        }

        public Environment LoadEnv()
        {
            var env = Load<Environment>(_filename);
            env.Initialize(_filename);

            return env;
        }

        /// <summary>
        /// Returns true if this is the user's first load.
        /// </summary>
        public bool IsFirstLoad()
            => !Exists();

        /// <summary>
        /// Creates the settings file with default settings.
        /// </summary>
        public void Create()
            => Save(GetDefaultSettings(), _filename);

        /// <summary>
        /// Creates the environment file with default settings.
        /// </summary>
        public void CreateEnv()
            => Save(GetDefaultEnvironment(), _filename);

        /// <summary>
        /// Gets the default environment.
        /// </summary>
        /// <returns></returns>
        public Environment GetDefaultEnvironment()
            => new Environment
            {
                Pipelines = new List<Settings>
                {
                    GetDefaultSettings("Pipeline 1"),
                    GetDefaultSettings("Pipeline 2")
                }
            };

        /// <summary>
        /// Gets the default settings.
        /// </summary>
        public Settings GetDefaultSettings(string pipelineName = "Default")
        {
            var defaultFiles = new string[2] { "appsettings.json", "web.config" };

            var settings = new Settings
            {
                Pipeline = new PipelineSettings
                {
                    Name = pipelineName,
                    Rollback = new RollbackSettings
                    {
                        Enabled = true,
                        BackupPath = "C:/Deployment/backup",
                    }
                },
                Git = new GitSettings
                {
                    Enabled = false,
                    FailOnError = true,
                    RepositoryPath = "C:/Folder/Repo",
                    AutoStash = true,
                    AutoPull = true,
                    Branch = "master"
                },
                Publish = new PublishSettings
                {
                    Enabled = false,
                    IgnoreRebuildErrors = false,
                    SolutionFile = "C:/Folder/Solution.sln",
                    ProjectFile = "C:/Folder/Project.csproj"
                },
                Source = new SourceSettings
                {
                    Zip = true,
                    Path = "C:/Folder/Source",
                    FilesToDelete = defaultFiles
                },
                Target = new TargetSettings
                {
                    Path = "C:/Deployment/results",
                    CleanTarget = true,
                    FilesToKeep = defaultFiles
                },
                Deployment = new DeploymentSettings
                {
                    Mode = DeployMode.Local,
                    Unzip = false,
                    IIS = new IISSettings
                    {
                        Enabled = false,
                        Host = "localhost",
                        AppPool = "MyAppPool"
                    },
                    Logging = new LoggerSettings
                    {
                        Enabled = true,
                        Mode = LoggerMode.File,
                        File = new FileLoggerSettings
                        {
                            FilenameFormat = "<pipeline>_[ddMMyyyy_HHmmss_fff].log",
                            LogPath = "logs/"
                        },
                        Sql = new SqlLoggerSettings
                        {
                            ConnectionString = "Server=myServer;Database=myDB;User Id=myUser;Password=myPass;"
                        }
                    }
                },
                Credentials = new Credentials
                {
                    IIS = new IISCredentials
                    {
                        Username = "",
                        Password = ""
                    },
                    NetworkDrive = new NetworkDriveCredentials
                    {
                        Username = "",
                        Password = ""
                    }
                },
                Extra = new ExtraSettings
                {
                    ShowHelp = true,
                    InteractiveMode = true,
                    FullscreenMode = true,
                    ShowSettings = true
                }
            };

            return settings;
        }
        #region Private Methods
        private bool Exists()
            => File.Exists(_filename);

        public static void Save<T>(T data, string filename)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filename, json);
        }

        public static T Load<T>(string filename)
        {
            string json = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<T>(json);
        }
        #endregion
    }
}
