using DeploymentTool.Base;
using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UIx = DeploymentTool.UI.UI;

namespace DeploymentTool.Services
{
    public class VisualStudioService : Loggable
    {
        private readonly Settings _settings;

        #region Constructor
        public VisualStudioService(Settings settings, ILogger logger) : base(logger)
            => _settings = settings;
        #endregion

        public async Task RebuildSolution()
        {
            if (!_settings.Publish.Enabled) return;

            await CleanSolution();

            string solution = _settings.Publish.SolutionFile;
            if (string.IsNullOrWhiteSpace(solution) || !File.Exists(solution))
                throw new FileNotFoundException($"Missing solution file: {solution}");

            Log($"Rebuilding solution {solution}...");

            await RunProcessWithFakeProgress(
                "dotnet",
                $"build \"{solution}\" --configuration Release",
                "Rebuilding solution..."
            );

            Log("Rebuilt.");
        }

        public async Task PublishProject()
        {
            if (!_settings.Publish.Enabled) return;

            string project = _settings.Publish.ProjectFile;
            if (string.IsNullOrWhiteSpace(project) || !File.Exists(project))
                throw new FileNotFoundException($"Missing project file: {project}");

            Log($"Publishing project {project}...");

            int maxRetries = 5;
            int delayMs = 500;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    KillStaleBuildProcesses();

                    await RunProcessWithRealProgress(
                        "dotnet",
                        $"publish \"{project}\" -c Release -o \"{_settings.Source.Path}\"",
                        "Publishing project..."
                    );

                    Log("Published successfully.");
                    return;
                }
                catch (Exception ex) when (ex.Message.Contains("Access to the path") || ex.Message.Contains("used by another process"))
                {
                    Log($"Attempt {attempt}/{maxRetries} failed due to locked DLLs. Retrying in {delayMs}ms...");
                    await Task.Delay(delayMs);
                }
            }

            throw new Exception($"Publishing project FAILED after {maxRetries} attempts due to locked DLLs.");
        }

        protected override void Log(string message)
            => _logger.Log("Visual Studio Service", message);

        #region Private Methods
        private void KillStaleBuildProcesses()
        {
            string[] processesToKill = { "msbuild", "VBCSCompiler", "dotnet" };

            foreach (var procName in processesToKill)
            {
                foreach (var proc in Process.GetProcessesByName(procName))
                {
                    try
                    {
                        if (!proc.HasExited)
                        {
                            Log($"Killing process {proc.ProcessName} (PID {proc.Id}) to release DLL locks...");
                            proc.Kill(true);
                            proc.WaitForExit(1000);
                        }
                    }
                    catch
                    {
                        // ignore if already exited or access denied
                    }
                }
            }
        }

        private async Task CleanSolution()
        {
            string solution = _settings.Publish.SolutionFile;
            if (string.IsNullOrWhiteSpace(solution) || !File.Exists(solution))
                throw new FileNotFoundException($"Missing solution file: {solution}");

            Log($"Cleaning solution {solution}...");

            await RunProcessWithRealProgress(
                "dotnet",
                $"clean \"{solution}\" -c Release",
                "Cleaning solution..."
            );

            Log("Cleaned.");
        }

        private async Task RunProcessWithRealProgress(string fileName, string args, string label)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            var vsOut = new ConcurrentQueue<string>();

            process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                    vsOut.Enqueue($"(StdOut) {e.Data}");
            };

            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                    vsOut.Enqueue($"(StdErr) {e.Data}");
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            while (!process.HasExited)
            {
                UIx.RenderProgress(50, $"{label}…", ConsoleColor.Magenta);
                await Task.Delay(200);
            }

            process.WaitForExit();

            while (vsOut.TryDequeue(out var line))
                Log(line);

            if (process.ExitCode != 0)
                throw new Exception($"{label} FAILED. ExitCode: {process.ExitCode}");

            UIx.RenderProgress(100, $"{label} (Finished)", ConsoleColor.Magenta);
        }

        private async Task RunProcessWithFakeProgress(string fileName, string args, string label)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = false,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            string? line;
            int progress = 0;

            while ((line = await process.StandardOutput.ReadLineAsync()) != null)
            {
                progress = Math.Min(progress + 3, 90);
                UIx.RenderProgress(progress, $"{label}…", ConsoleColor.Magenta);
            }

            string errorOut = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            progress = 100;
            UIx.RenderProgress(progress, $"{label} (Finishing)", ConsoleColor.Magenta);

            if (!string.IsNullOrWhiteSpace(errorOut))
                UIx.LogError(errorOut);

            if (process.ExitCode != 0)
                throw new Exception($"{label} FAILED. ExitCode: {process.ExitCode}");
        }
        #endregion
    }
}
