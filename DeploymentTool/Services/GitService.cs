using DeploymentTool.Base;
using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using DeploymentTool.Models.SettingsModules;
using System.Diagnostics;
using UIx = DeploymentTool.UI.UI;

namespace DeploymentTool.Services
{
    public class GitService : Loggable
    {
        private readonly GitSettings _settings;

        #region Constructor
        public GitService(Settings settings, ILogger logger) : base(logger)
        {
            _settings = settings?.Git ?? throw new ArgumentNullException(nameof(settings));
        }
        #endregion

        public void Execute()
        {
            if (!_settings.Enabled)
            {
                Log("Git is disabled in settings.");
                return;
            }

            UIx.RenderProgress(0, "Initializing Git...", ConsoleColor.DarkYellow);

            ValidateRepo();

            if (_settings.AutoStash) StashChanges();
            if (_settings.AutoPull) PullLatest();

            CheckBranch();

            UIx.RenderProgress(100, "Git operations completed.", ConsoleColor.DarkYellow);
        }

        /// <summary>
        /// Checks if the specified branch exists in the repository.
        /// </summary>
        public bool BranchExists(string branchName)
        {
            if (string.IsNullOrWhiteSpace(branchName))
                return false;

            try
            {
                string output = RunGit($"rev-parse --verify {branchName}");
                return !string.IsNullOrEmpty(output);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the specified path is a valid Git repository.
        /// </summary>
        public bool IsValidRepo(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            string gitDir = Path.Combine(path, ".git");
            return Directory.Exists(gitDir);
        }

        protected override void Log(string message)
            => _logger.Log("Git Service", message);

        #region Private Methods
        private void ValidateRepo()
        {
            Log("Validating repository...");

            if (string.IsNullOrWhiteSpace(_settings.RepositoryPath))
                throw new Exception("Git repo path is empty. Fix your damn config.");

            if (!Directory.Exists(Path.Combine(_settings.RepositoryPath, ".git")))
                throw new Exception("No .git directory found. Are you drunk? This isn't a repo.");

            Log("Repository is valid.");
            UIx.RenderProgress(10, "Valid Git repository detected.", ConsoleColor.DarkYellow);
        }

        private void StashChanges()
        {
            Log("Stasging changes...");
            UIx.RenderProgress(20, "Stashing changes...", ConsoleColor.DarkYellow);

            string result = RunGit("stash push --include-untracked");

            Log("Stashed.");
            UIx.RenderProgress(30, $"Stash completed", ConsoleColor.DarkYellow);
        }

        private void PullLatest()
        {
            Log("Pulling latest...");
            UIx.RenderProgress(40, "Pulling latest from Git...", ConsoleColor.DarkYellow);

            string result = RunGit("pull");

            Log("Pulled.");
            UIx.RenderProgress(70, $"Pull completed", ConsoleColor.DarkYellow);
        }

        private void CheckBranch()
        {
            if (string.IsNullOrWhiteSpace(_settings.Branch))
            {
                Log("Skipping branch check.");
                UIx.RenderProgress(80, "Skipping branch check.", ConsoleColor.DarkYellow);
                return;
            }

            Log("Checking branch...");
            UIx.RenderProgress(75, "Checking current branch...");

            var currentBranch = RunGit("rev-parse --abbrev-ref HEAD").Trim();

            if (!string.Equals(currentBranch, _settings.Branch, StringComparison.OrdinalIgnoreCase))
            {
                UIx.RenderProgress(75, $"Switching branch: {currentBranch} > {_settings.Branch}", ConsoleColor.DarkYellow);
                RunGit($"checkout {_settings.Branch}");
            }

            Log("Branch check done.");
            UIx.RenderProgress(85, $"Branch: {_settings.Branch}", ConsoleColor.DarkYellow);
        }

        private string RunGit(string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                WorkingDirectory = _settings.RepositoryPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var p = Process.Start(psi);
            if (p == null)
                throw new Exception("Git failed to start. Go reinstall Windows or something.");

            string output = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();

            p.WaitForExit();

            if (p.ExitCode != 0)
                throw new Exception($"Git died running '{arguments}':\n{error}");

            return output;
        }
        #endregion
    }
}
