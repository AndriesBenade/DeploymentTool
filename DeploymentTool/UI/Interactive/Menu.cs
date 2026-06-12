using DeploymentTool.Base;
using DeploymentTool.Interfaces;
using DeploymentTool.Models;
using DeploymentTool.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIx = DeploymentTool.UI.UI;

namespace DeploymentTool.UI.Interactive
{
    public abstract class Menu : Loggable
    {
        private const bool _colorModeEnabled = true;
        private const int _defaultWidth = 50;
        protected Settings _settings;

        public bool Active = true;
        protected string Title { get; private set; }
        protected List<MenuOption> Options { get; private set; }
        protected int Index { get; private set; }

        #region Constructor
        public Menu(Settings settings, string title, List<MenuOption> options, int startingIndex, ILogger logger) : base(logger)
        {
            _settings = settings;

            Title = title;
            Options = options;
            Index = startingIndex;
        }
        #endregion

        public virtual void Render()
        {
            while (Active)
            {
                UIx.Clear();
                
                if (!_colorModeEnabled)
                    UIx.RenderAdvancedDialog(Title, RenderOptions(), true, ConsoleColor.White);
                else
                    UIx.RenderAdvancedColorDialog(Title, RenderColorOptions(), true, ConsoleColor.White);

                ResolveKey(UIx.ReadKey());
            }
        }

        protected override void Log(string message)
            => _logger.Log("Menu", message);    

        #region Event Handlers
        public void Test() { }

        public void Exit()
            => Active = false;
        #endregion

        #region Protected Methods
        protected string[] RenderOptions()
        {
            List<string> options = new();

            foreach (var o in Options)
                options.Add(UIx.FillString(o.Render(Index), UIx.Empty, _defaultWidth));

            return options.ToArray();
        }

        protected List<KeyValuePair<string, ConsoleColor>> RenderColorOptions()
        {
            List<KeyValuePair<string, ConsoleColor>> options = new();

            foreach (var o in Options)
                options.Add(new(UIx.FillString(o.Render(Index), UIx.Empty, _defaultWidth), o.Color));

            return options;
        }

        protected void ResolveKey(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:    HandleUp(); break;
                case ConsoleKey.LeftArrow:  HandleUp(); break;

                case ConsoleKey.DownArrow:  HandleDown(); break;
                case ConsoleKey.RightArrow: HandleDown(); break;

                case ConsoleKey.Enter:      HandleSelect(); break;
                case ConsoleKey.Escape:     Exit(); break;
            }
        }

        protected int GetMaxIndex()
            => Options.Max(x => x.Index);

        protected int GetMinIndex()
            => Options.Min(x => x.Index);

        protected MenuOption GetOption(int index)
            => Options.FirstOrDefault(x => x.Index == index);
        #endregion

        #region Private Methods
        private void HandleUp()
            => ResolveIndex(Index--);

        private void HandleDown()
            => ResolveIndex(Index++);

        private void HandleSelect()
            => GetOption(Index).Invoke();

        private void ResolveIndex(int newIndex)
        {
            if (Index > GetMaxIndex()) Index = GetMinIndex();
            if (Index < GetMinIndex()) Index = GetMaxIndex();
        }
        #endregion
    }
}
