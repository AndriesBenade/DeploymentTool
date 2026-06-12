using DeploymentTool.Helpers;
using DeploymentTool.Models;
using DeploymentTool.Services;
using DeploymentTool.Services.Deployers;
using DeploymentTool.UI;
using DeploymentTool.UI.Interactive;
using DeploymentTool.UI.Menus;
using System.Runtime.CompilerServices;
using System.Security.Principal;

UI.Clear();

if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
    throw new InvalidOperationException("The Deployment Tool must be run as Administrator.");

var file = "appsettings.json";
var settingsService = new SettingsService(file);

if (settingsService.IsFirstLoad())
{
    settingsService.Create();

    UI.Log($"Settings file ({file}) has been created.\nConfigure the deployment via the file and once ready, run the Deployment Tool again.");
    UI.ReadKey();
    
    return;
}

Settings settings = settingsService.Load();

if (settings.Extra.FullscreenMode)
    ConsoleService.MaximizeConsole();

if (settings.Extra.ShowHelp)
{
    UI.Clear();
    SystemRenderer.RenderHelp();
    UI.LogStatus("Press any key to proceed...");
    UI.ReadKey();
}

if (settings.Extra.InteractiveMode)
{
    try
    {
        new MainMenu(settings, settings.GetLogger()).Render();
    }
    catch (Exception ex)
    {
        ErrorService.HandleError(ex, "Something went wrong!");
    }
}
else
{
    try
    {
        new DefaultService(settings.GetLogger()).Deploy("appsettings");
    }
    catch (Exception ex)
    {
        ErrorService.HandleError(ex, "Something went wrong!");
    }
}
UI.Log("Press any key to exit...");
UI.ReadKey();