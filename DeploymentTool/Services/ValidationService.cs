using DeploymentTool.Models;
using DeploymentTool.Models.SettingsModules;
using DeploymentTool.Services.Deployers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading.Tasks;
using Environment = DeploymentTool.Models.Environment;

namespace DeploymentTool.Services
{
    public class ValidationService
    {
        private List<string> _issues;

        public ValidationService()
            => Reset();

        /// <summary>
        /// Validates the data of an environment object.
        /// </summary>
        public bool ValidateEnvironment(Environment e, bool feedbackOnError = false)
        {
            Reset();
            UI.UI.Clear($"Validating environment ({e.GetName()})...");
            List<string> output = new();

            bool valid = true;
            int prevCount = 0;
            foreach (var p in e.Pipelines)
            {
                ValidatePipeline(p);

                if (prevCount != _issues.Count)
                {
                    output.Add($"{p.Pipeline.Name}");
                    output.AddRange(_issues.Select(i => $"  {i}"));
                    output.Add(string.Empty);

                    valid = false;
                    prevCount = _issues.Count;
                }
            }

            if (!valid)
                RenderIssues(output, e.GetName());
            else
            {
                if (!feedbackOnError)
                    RenderSuccess(e.GetName());
            }

            return valid;
        }

        /// <summary>
        /// Validates the data of a pipeline/settings object.
        /// </summary>
        public bool ValidatePipeline(Settings s, bool feedbackOnError = false)
        {
            Reset();
            UI.UI.Clear($"Validating pipeline ({s.Pipeline.Name})...");

            ValidatePipelineSettings(s.Pipeline);
            ValidateRollbackSettings(s.Pipeline.Rollback);
            ValidateGitSettings(s);
            ValidatePublishSettings(s.Publish);
            ValidateSourceSettings(s.Source);
            ValidateTargetSettings(s.Target);
            ValidateDeploymentSettings(s.Deployment);
            ValidateIISSettings(s);
            ValidateLoggerSettings(s.Deployment.Logging);

            bool valid = _issues.Count == 0;

            if (!valid)
                RenderIssues(_issues, s.Pipeline.Name);
            else
            {
                if (!feedbackOnError)
                    RenderSuccess(s.Pipeline.Name);
            }

            return valid;
        }

        #region Private Methods
        private void ValidatePipelineSettings(PipelineSettings p)
        {
            String(p.Name, "Pipeline > Name");
        }

        private void ValidateRollbackSettings(RollbackSettings r)
        {
            if (!r.Enabled)
                return;

            Path(r.BackupPath, "Rollback > Backup Path");
        }

        private void ValidateGitSettings(Settings s)
        {
            if (!s.Git.Enabled)
                return;

            GitBranch(s.Git.Branch, "Git > Branch", s);
            GitRepo(s.Git.RepositoryPath, "Git > Repository Path", s);
        }

        private void ValidatePublishSettings(PublishSettings p)
        {
            if (!p.Enabled)
                return;

            File(p.SolutionFile, "Publish > Solution File", ".sln");
            File(p.ProjectFile, "Publish > Project File", ".csproj");
        }

        private void ValidateSourceSettings(SourceSettings s)
        {
            Path(s.Path, "Source > Path");
        }

        private void ValidateTargetSettings(TargetSettings t)
        {
            Path(t.Path, "Target > Path");
        }

        private void ValidateDeploymentSettings(DeploymentSettings d)
        {
            if (d.Mode == DeployMode.NetworkDrive)
                Issue("Network drive deployment mode is no longer supported");
        }

        private void ValidateIISSettings(Settings s)
        {
            if (!s.Deployment.IIS.Enabled)
                return;

            String(s.Deployment.IIS.Host, "IIS > Host");
            IISAppPool(s.Deployment.IIS.AppPool, "IIS > App Pool", s);
        }

        private void ValidateLoggerSettings(LoggerSettings l)
        {
            switch (l.Mode)
            {
                case LoggerMode.File:

                    String(l.File.FilenameFormat, "Logging > File > Filename Format");
                    String(l.File.LogPath, "Logging > File > Log Path");

                    break;
                case LoggerMode.Sql:

                    SqlConnection(l.Sql.ConnectionString, "Logging > SQL > Connection String");

                    break;
            }
        }
        #endregion

        #region Helpers
        private void String(string value, string context)
        {
            if (string.IsNullOrEmpty(value))
                Issue($"{context}: Value cannot be empty");
        }

        private void Path(string value, string context)
        {
            if (string.IsNullOrEmpty(value))
            {
                Issue($"{context}: Path cannot be empty");
                return;
            }

            if (!Directory.Exists(value))
                Issue($"{context}: Path does not exist");
        }

        private void File(string value, string context, string ext = "")
        {
            if (string.IsNullOrEmpty(value))
            {
                Issue($"{context}: File cannot be empty");
                return;
            }

            if (!string.IsNullOrEmpty(ext) && !value.ToLower().EndsWith(ext.ToLower()))
                Issue($"{context}: File has to be of type ({ext})");

            if (!System.IO.File.Exists(value))
                Issue($"{context}: File does not exist");
        }

        private void SqlConnection(string connectionString, string context)
        {
            if (string.IsNullOrEmpty(connectionString))
                Issue($"{context}: Connection string cannot be empty");

            if (!new SqlService(connectionString).TestConnection(out Exception ex))
            {
                Issue($"{context}: Unable to connect using the provided connection string");
                ErrorService.HandleError(ex, $"{context}: SQL Connection Test Failed");
            }
        }

        private void IISAppPool(string appPool, string context, Settings s)
        {
            if (string.IsNullOrEmpty(appPool))
                Issue($"{context}: App pool cannot be empty");

            if (!new IISService(s.Deployment.IIS.Host, s.GetLogger(), s.Credentials.IIS).AppPoolExists(appPool))
            {
                Issue($"{context}: App pool not found on host");
            }
        }

        private void GitBranch(string branch, string context, Settings s)
        {
            if (string.IsNullOrEmpty(branch))
                Issue($"{context}: Branch cannot be empty");

            if (!new GitService(s, s.GetLogger()).BranchExists(branch))
            {
                Issue($"{context}: Branch does not exist");
            }
        }

        private void GitRepo(string repoPath, string context, Settings s)
        {
            Path(repoPath, context);

            if (!new GitService(s, s.GetLogger()).IsValidRepo(repoPath))
            {
                Issue($"{context}: Repo path is not a repository");
            }
        }

        private void Issue(string issue)
            => _issues.Add($"{UI.UI.Bullet} {issue}");

        private void Text(string text)
            => _issues.Add(text);

        private void Reset()
            => _issues = new();

        private void RenderIssues(List<string> issues, string name)
        {
            UI.UI.Clear();
            UI.UI.RenderAdvancedDialog($"{name} (Issues)", issues.ToArray(), true, ConsoleColor.Red);
            UI.UI.Log("Press any key to continue...");
            UI.UI.ReadKey();
        }

        private void RenderSuccess(string name)
        {
            UI.UI.Clear();
            UI.UI.RenderAdvancedDialog($"{name}", "No issues were found.", 70, true, ConsoleColor.Green);
            UI.UI.Log("Press any key to continue...");
            UI.UI.ReadKey();
        }
        #endregion
    }
}
