using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIx = DeploymentTool.UI.UI;

namespace DeploymentTool.UI.Interactive
{
    public class MenuOption
    {
        public int Index { get; set; }
        public string Caption { get; set; }
        public Action EventHandler { get; set; }
        public ConsoleColor Color { get; set; }

        #region Constructors
        public MenuOption(int index, string caption, Action eventHandler, ConsoleColor color = ConsoleColor.White)
        {
            Index = index;
            Caption = caption;
            EventHandler = eventHandler;
            Color = color;
        }
        #endregion

        /// <summary>
        /// Invokes the options event handler.
        /// </summary>
        public void Invoke()
            => EventHandler.Invoke();

        /// <summary>
        /// Returns the render of the option based on the selected index.
        /// </summary>
        public string Render(int index)
        {
            var bullet = index == Index ? UIx.Bullet : UIx.Empty;
            return $"{bullet} {Caption}";
        }
    }
}
