using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using DeploymentTool.Services;
using DeploymentTool.UI.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.UI.Menus
{
    public class SettingsMenu : Menu
    {
        #region Constructor
        public SettingsMenu(Settings settings, ILogger logger, int index = 0) : base(settings, "Settings", new List<MenuOption>(), index, logger)
        {
            Options.Add(new MenuOption(0, $"Show Help [{settings.Extra.ShowHelp}]", () => ToggleShowHelp()));
            Options.Add(new MenuOption(1, $"Interactive Mode [{settings.Extra.InteractiveMode}]", () => ToggleInteractiveMode()));
            Options.Add(new MenuOption(2, $"Fullscreen Mode [{settings.Extra.FullscreenMode}]", () => ToggleFullscreenMode()));
            Options.Add(new MenuOption(3, $"Show Settings [{settings.Extra.ShowSettings}]", () => ToggleShowSettings()));
            Options.Add(new MenuOption(4, "Back", () => Exit(), ConsoleColor.Red));
        }
        #endregion

        #region Event Handlers
        public void ToggleShowHelp()
        {
            _settings.Extra.ShowHelp = !_settings.Extra.ShowHelp;
            SaveAndRefresh();
        }

        public void ToggleInteractiveMode()
        {
            _settings.Extra.InteractiveMode = !_settings.Extra.InteractiveMode;
            SaveAndRefresh();
        }

        public void ToggleFullscreenMode()
        {
            _settings.Extra.FullscreenMode = !_settings.Extra.FullscreenMode;
            SaveAndRefresh();
        }
        public void ToggleShowSettings()
        {
            _settings.Extra.ShowSettings = !_settings.Extra.ShowSettings;
            SaveAndRefresh();
        }
        #endregion

        #region Private Methods
        private void SaveAndRefresh()
        {
            SettingsService.Save(_settings, _settings.GetFilename());
            new SettingsMenu(_settings, _logger, Index).Render();
            Exit();
        }
        #endregion
    }
}
