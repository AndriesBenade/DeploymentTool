using DeploymentTool.Helpers;
using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using DeploymentTool.Models.SettingsModules;
using DeploymentTool.Services;
using DeploymentTool.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Environment = DeploymentTool.Models.Environment;
using UIx = DeploymentTool.UI.UI;

namespace DeploymentTool.Base
{
    public abstract class Deployer : Loggable, IDeployer
    {
        private const int _uxDelay = 500;

        protected string _path;
        protected DeploymentType _type;

        #region Constructors
        public Deployer(ILogger logger, string path = "pipelines/", DeploymentType type = DeploymentType.Pipeline) : base(logger)
            => Initialize(path, type);
        #endregion

        public virtual void Deploy(string name)
        {
            if (!ConfirmDeployment(name)) return;

            switch (_type)
            {
                case DeploymentType.Pipeline: DeployPipeline(name); break;
                case DeploymentType.Environment: DeployEnvironment(name); break;
            }
        }

        public virtual void Deploy(Environment env)
        {
            if (_type != DeploymentType.Environment)
                throw new Exception("This deployment method only supports Environment deployments!");

            if (!ConfirmDeployment(env.GetName())) return;

            DeployEnvironment(env).Wait();
        }

        /// <summary>
        /// Gets the list of available files for the selected deployment type.
        /// </summary>
        public List<string> GetFiles()
        {
            List<string> files = Directory.GetFiles(_path, "*.json", SearchOption.TopDirectoryOnly)
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToList();

            return files;
        }

        /// <summary>
        /// Gets the object based on deployment type.
        /// </summary>
        public dynamic Get(string name)
            => _type switch
            {
                DeploymentType.Pipeline => GetPipeline(name),
                DeploymentType.Environment => GetEnvironment(name),
                _ => throw new Exception("Unrecognized deployment type!")
            };

        protected override void Log(string message)
            => _logger.Log("Deployer", message);

        #region Protected Methods
        protected Settings GetPipeline(string name)
            => new SettingsService(GetFilePath(name)).Load();

        protected Environment GetEnvironment(string name)
            => new SettingsService(GetFilePath(name)).LoadEnv();

        protected void DeployEnvironment(string name)
        {
            var env = GetEnvironment(name);
            DeployEnvironment(env).Wait();
        }

        protected async Task DeployEnvironment(Environment env)
        {
            Log("Validating environment pipelines...");
            if (!new ValidationService().ValidateEnvironment(env, true))
            {
                Log("Environment pipelines validation failed. Deployment aborted.");
                return;
            }

            Log("Environment pipelines validated.");

            bool result = true;
            List<int> history = new();

            Log("Starting environment deployment...");

            for (int i = 0; i < env.Pipelines.Count; i++)
            {
                var p = env.Pipelines[i];
                Log($"Deploying environment pipeline ({p.Pipeline.Name})...");

                UIx.Clear();
                if (p.Extra.ShowSettings)
                    SystemRenderer.RenderSettings(p);

                string releaseName = GetReleaseName(p);

                UIx.Clear();
                var deployer = new DeploymentService(p, releaseName);

                if (!await deployer.Deploy())
                {
                    result = false;
                    break;
                }
                else
                {
                    Log($"Environment pipeline ({p.Pipeline.Name}) deployed successfully.");
                    history.Add(i);
                }
            }

            if (!result)
            {
                UIx.Clear("Environment deployment failed!", _uxDelay);
                UIx.Clear("Preparing to rollback environment pipelines...", _uxDelay);

                foreach (int i in history)
                {
                    var p = env.Pipelines[i];
                    Log($"Rolling back environment pipeline ({p.Pipeline.Name})...");

                    new RollbackService(p, _logger).Rollback().Wait();
                    Log($"Environment pipeline ({p.Pipeline.Name}) rolled back successfully.");
                }

                UIx.Clear("Environment rollback completed.", _uxDelay);
            }
        }

        protected void DeployPipeline(string name)
        {
            Log("Validating pipeline...");
            if (!new ValidationService().ValidatePipeline(GetPipeline(name), true))
            {
                Log("Pipeline validation failed. Deployment aborted.");
                return;
            }

            Log("Pipeline validated.");
            Log("Starting pipeline deployment...");
            var settings = GetPipeline(name);

            UIx.Clear();
            if (settings.Extra.ShowSettings)
                SystemRenderer.RenderSettings(settings);

            string releaseName = GetReleaseName(settings);

            UIx.Clear();
            var deployer = new DeploymentService(settings, releaseName);
            deployer.Deploy().Wait();
            Log("Pipeline deployment completed.");
        }

        protected string GetFilePath(string name)
            => Path.Combine(_path, $"{name}.json");
        #endregion

        #region Private Methods
        private void Initialize(string path, DeploymentType type)
        {
            _path = path;
            _type = type;

            if (!string.IsNullOrEmpty(path))
            {
                if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);
                if (Directory.GetFiles(_path, "*.json").Length > 0) return;
            }

            switch (_type)
            {
                case DeploymentType.Pipeline: InitPipeline(); break;
                case DeploymentType.Environment: InitEnv(); break;
            }
        }

        private void InitPipeline()
            => new SettingsService(GetFilePath("My Pipeline")).Create();

        private void InitEnv()
            => new SettingsService(GetFilePath("My Environment")).CreateEnv();

        private string GetReleaseName(Settings settings)
        {
            if (settings.Deployment.Mode != DeployMode.Local || settings.Source.Zip)
            {
                UIx.Clear();
                UIx.Log($"The '{settings.Pipeline.Name}' pipeline has Zip enabled for source.");
                UIx.Log("A release name is required for improved release management.");
                UIx.Log();

                var files = settings.Target.GetFilesInTarget();
                if (files.Length > 0)
                {
                    UIx.Log("These are the files that already exist in the target folder:");

                    foreach (var file in files)
                        UIx.Log($"\t{UIx.Bullet} {Path.GetFileName(file)}");
                }
                else
                    UIx.Log("The are no existing files in the target folder.");

                UIx.Log();
                return UIx.ReadLine("Release Name");
            }
            else
                return string.Empty;
        }

        private bool ConfirmDeployment(string name)
        {
            Log("Confirming deployment...");
            var result = UIx.Confirmation($"Confirm Deployment ({name})", new List<string> { $"Are you sure you want to proceed with this {_type.ToString().ToLower()} deployment?", "Once the process starts, it cannot be stopped/cancelled." });

            if (result) Log("Deployment confirmed.");
            else Log("Deployment cancelled by user.");

            return result;
        }
        #endregion
    }

    public enum DeploymentType
    {
        Pipeline = 0,
        Environment = 1
    }
}
