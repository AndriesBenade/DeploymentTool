using DeploymentTool.Base;
using DeploymentTool.Models;
using DeploymentTool.Models.SettingsModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIx = DeploymentTool.UI.UI;

namespace DeploymentTool.Helpers
{
    public static class SystemRenderer
    {
        #region Help
        public static void RenderHelp()
        {
            UIx.RenderAdvancedDialog(
                "Help [1/9]",
                new List<string>
                {
                    $"Pipeline",
                    $"    ■ Name: Name of the pipeline",
                    $"    ■ Rollback",
                    $"        ■ Enabled: Enables/disables automatic rollbacks and backup creation",
                    $"        ■ BackupPath: Full path to the folder where backups will be stored",
                }
                .ToArray(),
                true,
                ConsoleColor.White
            );
            UIx.ReadKey();

            UIx.RenderAdvancedDialog(
                "Help [2/9]",
                new List<string>
                {
                    $"Git",
                    $"    ■ Enabled: Enables/disables automated git operations",
                    $"    ■ Fail On Error: Stops deployment if any errors occur during git operations if true",
                    $"    ■ Repository Path: Full path to where the repository is located",
                    $"    ■ Branch: Name of the branch to deploy from",
                    $"    ■ Auto Stash: Auto stashes any changes if enabled",
                    $"    ■ Auto Pull: Auto pulls latest for specified branch if enabled",
                }
                .ToArray(),
                true,
                ConsoleColor.White
            );
            UIx.ReadKey();

            UIx.RenderAdvancedDialog(
                "Help [3/9]",
                new List<string>
                {
                    $"Publish",
                    $"    ■ Enabled: Enables/disables visual studio rebuilding and publishing",
                    $"    ■ Ignore Rebuild Errors: Stops deployment on any rebuild errors if false",
                    $"    ■ Solution File: Full path to the solution (.sln) file for rebuilding",
                    $"    ■ Project File: Full path to the project (.csproj) file for publishing",
                }
                .ToArray(),
                true,
                ConsoleColor.White
            );
            UIx.ReadKey();

            UIx.RenderAdvancedDialog(
                "Help [4/9]",
                new List<string>
                {
                    $"Source",
                    $"    ■ Zip: Enables/disables zipping of source in deployment",
                    $"    ■ Path: Full source path",
                    $"    ■ Files To Delete: List of files to be deleted from source before starting deployment",
                }
                .ToArray(),
                true,
                ConsoleColor.White
            );
            UIx.ReadKey();

            UIx.RenderAdvancedDialog(
                "Help [5/9]",
                new List<string>
                {
                    $"Target",
                    $"    ■ Path: Full target path (use only paths created by this app or paths that dont exist yet",
                    $"          and the path will be created automatically. This is to avoid any permission issues)",
                    $"    ■ Clean Target: Enables/disables cleanup of target folder before deployment",
                    $"    ■ Files To Keep: Files to not delete in target folder during target cleanup",
                }
                .ToArray(),
                true,
                ConsoleColor.White
            );
            UIx.ReadKey();

            UIx.RenderAdvancedDialog(
                "Help [6/9]",
                new List<string>
                {
                    $"Deployment",
                    $"    ■ Mode: Deployment mode [Local = 0, Network Drive = 1]",
                    $"    ■ Unzip: Enables/disables automatic unzipping of zip file in target if Source.Zip is true",
                    $"    ■ IIS",
                    $"        ■ Enabled: Enables/disables automatic app pool management during deployment",
                    $"        ■ Host: Host for IIS [default = localhost]",
                    $"        ■ App Pool: Name of app pool",
                    $"    ■ Logging",
                    $"        ■ Enabled: Enables/disables logging during deployment",
                    $"        ■ Mode: Logger mode [File = 0, Sql = 1]",
                    $"        ■ File",
                    $"            ■ FilenameFormat: Format of log file names [default = <pipeline>_[ddMMyyyy_HHmmss_fff].log]",
                    $"            ■ LogPath: Folder where logs should be stored",
                    $"        ■ Sql",
                    $"            ■ ConnectionString: Connection string for database logging",
                }
                .ToArray(),
                true,
                ConsoleColor.White
            );
            UIx.ReadKey();

            UIx.RenderAdvancedDialog(
                "Help [7/9]",
                new List<string>
                {
                    $"Credentials",
                    $"    ■ IIS",
                    $"        ■ Username: Username to be used for remote IIS management",
                    $"        ■ Password: Password to be used for remote IIS management",
                    $"    ■ Network Drive",
                    $"        ■ Username: Username to be used to map network drive",
                    $"        ■ Password: Password to be used to map network drive",
                }
                .ToArray(),
                true,
                ConsoleColor.White
            );
            UIx.ReadKey();

            UIx.RenderAdvancedDialog(
                "Help [8/9]",
                new List<string>
                {
                    $"Extra",
                    $"    ■ Show Help: Enables/disables showing this help menu on startup",
                    $"    ■ Interactive Mode: Enables/disables starting the console up in interactive mode",
                    $"    ■ Fullscreen Mode: Enables/disables starting the console up in fullscreen mode",
                    $"    ■ Show Settings: Enables/disables showing pipeline settings before each deployment",
                }
                .ToArray(),
                true,
                ConsoleColor.White
            );
            UIx.ReadKey();

            UIx.RenderAdvancedDialog(
                "Help [9/9]",
                new List<string>
                {
                    $"Note that when not starting the console up in interactive mode, that the default",
                    $"appsettings.json pipeline configuration will be used for deployment.",
                }
                .ToArray(),
                true,
                ConsoleColor.White
            );
            UIx.ReadKey();

            UIx.RenderAdvancedDialog(
                "Help (Pipeline Management)",
                new List<string>
                {
                    $"You are able to have multiple deployment pipelines setup. To set these up, you can",
                    $"navigate to the 'pipelines' folder located in the same folder as appsettings.json",
                    $"",
                    $"The pipeline files are in the exact same format and layout as appsettings.json so",
                    $"you can easily copy/paste to add new pipelines. Ensure you provide each pipeline",
                    $"file with an appropriate name for display in the UI.",
                }
                .ToArray(),
                true,
                ConsoleColor.White
            );
            UIx.ReadKey();
        }

        public static void RenderOldHelp()
        {
            UIx.RenderAdvancedDialog(
                "Help",
                new List<string>
                {
                    $"Modes",
                    $"    ■ Local (0)",
                    $"    ■ Network Drive (1)",
                    $"",
                    $"Paths",
                    $"    ■ All paths need to use forward slashes (/) instead or escaped",
                    $"          escaped backward slashes (\\\\)",
                    $"",
                    $"Internet Information Services (IIS)",
                    $"    ■ Automatically stops/starts/recycles app pools during deployment if",
                    $"          enabled in settings",
                    $"",
                    $"Publishing",
                    $"    ■ To automate rebUIxlding/publishing, you need to enable it in settings",
                    $"    ■ Ensure the source folder is the folder the project will be published in",
                    $"",
                    $"Zipping",
                    $"    ■ Files are zipped before copy if 'Zip' is enabled in source settings",
                    $"    ■ You will be prompted for a release name within the console",
                    $"    ■ You can specify whether the file needs to be unzipped in the target folder",
                    $"          by toggling 'Unzip' in settings",
                    $"",
                    $"Credentials",
                    $"    ■ If credentials are left empty, the console assumes the target reqUIxres",
                    $"           no credentials",
                    $"",
                    $"Tip",
                    $"    ■ This dialog won't go away until you disable 'ShowHelp' in settings",
                }
                .ToArray(),
                true,
                ConsoleColor.White
            );
        }
        #endregion

        #region Settings
        public static void RenderSettings(Settings s)
        {
            RenderPipeline(s);
            UIx.ReadKey();

            RenderGit(s);
            UIx.ReadKey();

            RenderPublishing(s);
            UIx.ReadKey();

            UIx.RenderAdvancedDialog(
                "Source",
                new List<string>
                {
                    $"■ {s.Source.Path}",
                    $"■ {s.Source.FilesToDelete.Length} files will be deleted",
                    s.Source.Zip ? $"■ Source will be zipped" : $"■ Source will not be zipped"
                }
                .ToArray(),
                false
            );
            UIx.ReadKey();

            UIx.RenderAdvancedDialog(
                "Target",
                new List<string>
                {
                    $"■ {s.Target.Path}",
                    $"■ {s.Target.FilesToKeep.Length} files will be kept"
                }
                .ToArray(),
                false
            );
            UIx.ReadKey();

            UIx.RenderAdvancedDialog(
                "Deployment",
                new List<string>
                {
                    $"■ {s.Deployment.Mode.ToString()} Mode",
                    s.Deployment.Unzip ? "■ Auto Unzip Enabled" : "■ Auto Unzip Disabled"
                }
                .ToArray(),
                false
            );
            UIx.ReadKey();

            RenderIIS(s);
            UIx.ReadKey();

            RenderLogging(s);
            UIx.ReadKey();

            RenderCredentials(s);
            UIx.ReadKey();

            UIx.Log();
        }

        private static void RenderPipeline(Settings s)
        {
            UIx.RenderAdvancedDialog(
                "Pipeline",
                new List<string>
                {
                    $"■ Name: {s.Pipeline.Name}",
                }
                .ToArray(),
                false,
                ConsoleColor.White
            );
            UIx.ReadKey();

            if (s.Pipeline.Rollback.Enabled)
            {
                UIx.RenderAdvancedDialog(
                    "Rollback",
                    new List<string>
                    {
                        $"■ Enabled",
                        $"■ Backup Path: {s.Pipeline.Rollback.BackupPath}",
                    }
                    .ToArray(),
                    false,
                    ConsoleColor.White
                );
            }
            else
            {
                UIx.RenderAdvancedDialog(
                    "Rollback",
                    new List<string>
                    {
                        $"■ Disabled"
                    }
                    .ToArray(),
                    false,
                    ConsoleColor.DarkGray
                );
            }
        }

        private static void RenderGit(Settings s)
        {
            if (s.Git.Enabled)
            {
                UIx.RenderAdvancedDialog(
                    "Git",
                    new List<string>
                    {
                        $"■ Enabled",
                        s.Git.FailOnError ? $"■ Failing on error" : $"■ Not failing on error",
                        $"■ Repository: {Path.GetFileName(s.Git.RepositoryPath)}",
                        $"■ Branch: {s.Git.Branch}",
                        s.Git.AutoStash ? $"■ Auto stashing changes" : $"■ Not auto stashing changes",
                        s.Git.AutoPull ? $"■ Auto pulling latest" : $"■ Not auto pulling latest"
                    }
                    .ToArray(),
                    false,
                    ConsoleColor.Yellow
                );
            }
            else
            {
                UIx.RenderAdvancedDialog(
                    "Git",
                    new List<string>
                    {
                        $"■ Disabled"
                    }
                    .ToArray(),
                    false,
                    ConsoleColor.DarkYellow
                );
            }
        }

        private static void RenderPublishing(Settings s)
        {
            if (s.Publish.Enabled)
            {
                UIx.RenderAdvancedDialog(
                    "Publishing (Visual Studio)",
                    new List<string>
                    {
                        $"■ Enabled",
                        s.Publish.IgnoreRebuildErrors ? $"■ Ignoring rebuild errors" : $"■ Not ignoring rebuild errors",
                        $"■ Solution: {Path.GetFileName(s.Publish.SolutionFile)}",
                        $"■ Project: {Path.GetFileName(s.Publish.ProjectFile)}"
                    }
                    .ToArray(),
                    false,
                    ConsoleColor.Magenta
                );
            }
            else
            {
                UIx.RenderAdvancedDialog(
                    "Publishing (Visual Studio)",
                    new List<string>
                    {
                        $"■ Disabled"
                    }
                    .ToArray(),
                    false,
                    ConsoleColor.DarkMagenta
                );
            }
        }

        private static void RenderIIS(Settings s)
        {
            if (s.Deployment.IIS.Enabled)
            {
                UIx.RenderAdvancedDialog(
                    "Internet Information Services (IIS)",
                    new List<string>
                    {
                        $"■ Enabled"
                    }
                    .ToArray(),
                    false
                );
            }
            else
            {
                UIx.RenderAdvancedDialog(
                    "Internet Information Services (IIS)",
                    new List<string>
                    {
                        $"■ Disabled"
                    }
                    .ToArray(),
                    false,
                    ConsoleColor.DarkGray
                );
            }
        }

        private static void RenderLogging(Settings s)
        {
            if (!s.Deployment.Logging.Enabled)
            {
                UIx.RenderAdvancedDialog(
                    "Logging",
                    new List<string>
                    {
                        $"■ Disabled"
                    }
                    .ToArray(),
                    false,
                    ConsoleColor.DarkGray
                );
            }
            else
            {
                UIx.RenderAdvancedDialog(
                    "Logging",
                    new List<string>
                    {
                        $"■ Enabled",
                        $"■ Mode: {s.Deployment.Logging.Mode}"
                    }
                    .ToArray(),
                    false
                );
            }
        }

        private static void RenderCredentials(Settings s)
        {
            var creds = s.GetCredentials();
            if ((creds == null || !creds.HasCredentials()) && s.Deployment.Mode != DeployMode.Local)
            {
                UIx.RenderDialog("Assuming no credentials are reqUIxred", false, ConsoleColor.Gray);
                return;
            }

            if (creds is BasicCredentials)
            {
                UIx.RenderAdvancedDialog(
                    "Credentials",
                    new List<string>
                    {
                        $"■ {(creds as BasicCredentials).Username}",
                        $"■ {UIx.Passwordify((creds as BasicCredentials).Password)}"
                    }
                    .ToArray(),
                    false
                );
            }

            switch (s.Deployment.Mode)
            {
                case DeployMode.Local:
                    UIx.RenderDialog("Local mode has no credentials", false);
                    break;
            }
        }
        #endregion
    }
}
