using DeploymentTool.Base;
using DeploymentTool.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILogger = DeploymentTool.Interfaces.ILogger;
using UIx = DeploymentTool.UI.UI;

namespace DeploymentTool.Services
{
    public static class ErrorService
    {
        /// <summary>
        /// Handles the error systematically with logging.
        /// </summary>
        public static void HandleError(Exception ex, string contextMessage, ILogger logger)
        {
            UIx.Clear();
            UIx.Log($"{contextMessage}\n");

            Log($"An error occured: {contextMessage}", logger);
            Log($"Original Exception: {ex.Message}", logger);
            LogErrorInfo("Original Exception", ex, logger);

            UIx.RenderAdvancedDialog("Original Exception", ex.Message, 70, true, ConsoleColor.Red);

            if (ex.InnerException != null)
            {
                UIx.RenderAdvancedDialog("Inner Exception", ex.InnerException.Message, 70, true, ConsoleColor.DarkRed);

                Log($"Inner Exception: {ex.InnerException.Message}", logger);
                LogErrorInfo("Inner Exception", ex.InnerException, logger);
            }

            UIx.RenderAdvancedDialog(
                "Tip",
                new List<string>
                {
                    $"{UIx.Bullet} This console should automatically run with administrator privilages",
                    $"{UIx.Bullet} Ensure the site you are deploying to is not open in a browser somewhere"
                }
                .ToArray(),
                true,
                ConsoleColor.Yellow
            );
        }

        /// <summary>
        /// Handles the error systematically without logging.
        /// </summary>
        public static void HandleError(Exception ex, string contextMessage)
        {
            UIx.Clear();
            UIx.Log($"{contextMessage}\n");

            UIx.RenderAdvancedDialog("Original Exception", ex.Message, 70, true, ConsoleColor.Red);

            if (ex.InnerException != null)
                UIx.RenderAdvancedDialog("Inner Exception", ex.InnerException.Message, 70, true, ConsoleColor.DarkRed);

            UIx.RenderAdvancedDialog(
                "Tip",
                new List<string>
                {
                    $"{UIx.Bullet} This console should automatically run with administrator privelages",
                    $"{UIx.Bullet} Ensure the site you are deploying to is not open in a browser somewhere",
                    $"{UIx.Bullet} Folder permissions can be annoying, if you are experiencing them assign a path that does not yet exist",
                    $"      or delete the target folder and try again",
                }
                .ToArray(),
                true,
                ConsoleColor.Yellow
            );

            UIx.Log("Press [space] to continue...");
            UIx.ReadKey(ConsoleKey.Spacebar);
        }

        #region Private Methods
        private static void Log(string message, ILogger logger)
            => logger.Log("Error Service", message);

        private static void LogErrorInfo(string prefix, Exception ex, ILogger logger)
        {
            Log($"{prefix} Stack Trace: {ex.StackTrace}", logger);
            Log($"{prefix} Target Site: {ex.TargetSite}", logger);
            Log($"{prefix} Source: {ex.Source}", logger);
        }
        #endregion
    }
}
