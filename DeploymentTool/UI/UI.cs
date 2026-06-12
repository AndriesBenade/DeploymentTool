using DeploymentTool.Base;
using DeploymentTool.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using Spectre.Console.Rendering;

namespace DeploymentTool.UI
{
    public static class UI
    {
        #region Constants
        public static string Bullet => "■";
        public static string Empty => " ";
        #endregion

        #region Input
        public static ConsoleKeyInfo ReadKey()
            => Console.ReadKey();

        public static void ReadKey(ConsoleKey key)
        {
            while (ReadKey().Key != key);
        }

        public static string ReadLine()
            => Console.ReadLine();

        public static string ReadLine(string prompt)
        {
            Log($"{prompt}: ", true);
            return ReadLine();
        }
        #endregion

        #region Basic
        public static void Log(string message = "", bool lineMode = false)
        {
            if (lineMode)
                LogFullInLine(message);
            else
                LogFull(message);
        }

        public static void LogError(string message)
            => LogFull(message, ConsoleColor.Red);

        public static void LogPrompt(string message)
            => LogFull(message, ConsoleColor.Green);

        public static void LogStatus(string message)
            => LogFull(message, ConsoleColor.DarkCyan);

        public static void LogFull(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void LogFullInLine(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }
        #endregion

        #region Advanced
        public static bool Confirmation(string title, List<string> lines)
        {
            var key = ConsoleKey.X;
            while (key != ConsoleKey.Y && key != ConsoleKey.N)
            {
                Clear();

                var colorLines = new List<KeyValuePair<string, ConsoleColor>>();
                foreach (var l in lines)
                    colorLines.Add(new(l, ConsoleColor.White));

                colorLines.Add(new("    [Y] Yes", ConsoleColor.Green));
                colorLines.Add(new("    [N] No", ConsoleColor.Red));

                RenderAdvancedColorDialog(
                    title,
                    colorLines,
                    true,
                    ConsoleColor.White
                );

                key = ReadKey().Key;
            }

            switch (key)
            {
                case ConsoleKey.Y: return true;
                default: return false;
            }
        }

        public static void Clear(string initialMessage, int uxPauseDuration)
            => Clear(initialMessage, true, uxPauseDuration);

        /// <summary>
        /// Clears the console but keeps the title.
        /// </summary>
        public static void Clear(string initialMessage = "", bool uxPause = false, int uxPauseDuration = 100)
        {
            Console.Clear();

            string title = $"Deployment Tool v{Settings.Version}";
            Console.Title = title;
            UI.RenderSpecialDialog(title, true, ConsoleColor.Cyan);

            if (!string.IsNullOrEmpty(initialMessage))
                UI.Log(initialMessage);

            if (uxPause)
                Thread.Sleep(uxPauseDuration);
        }

        /// <summary>
        /// Renders a progress bar with a status.
        /// </summary>
        /// <param name="position">Position between 0 and 100 to indicate progress</param>
        /// <param name="status">Status text for progress bar status</param>
        public static void RenderProgress(int position, string status)
            => RenderProgress(position, status, ConsoleColor.DarkCyan);

        /// <summary>
        /// Renders a progress bar with a status.
        /// </summary>
        /// <param name="position">Position between 0 and 100 to indicate progress</param>
        /// <param name="status">Status text for progress bar status</param>
        public static void RenderProgress(int position, string status, ConsoleColor color)
        {
            int progressBarTop = 4;
            Console.SetCursorPosition(0, progressBarTop);

            UI.Log($" {FillString(SizeString(status, 100), " ", 100)}");

            int max = 100;
            string line = MultiplyString("─", max);
            string progress = FillString(MultiplyString("▓", position), "░", max);

            UI.LogFull($"┌─{line}─┐", color);
            UI.LogFull($"│ {progress} │", color);
            UI.LogFull($"└─{line}─┘", color);
            UI.Log();
        }

        /// <summary>
        /// Renders a basic wrap around dialog.
        /// </summary>
        public static void RenderDialog(string text, bool tailingLine, ConsoleColor color = ConsoleColor.White)
        {
            int max = text.Length + 2;
            string line = MultiplyString("─", max);
            string message = $" {text} ";

            UI.LogFull($"┌{line}┐", color);
            UI.LogFull($"│{message}│", color);
            UI.LogFull($"└{line}┘", color);

            if (tailingLine)
                UI.Log();
        }

        /// <summary>
        /// Renders a basic wrap around dialog with double borders.
        /// </summary>
        public static void RenderSpecialDialog(string text, bool tailingLine, ConsoleColor color = ConsoleColor.White)
        {
            int max = text.Length + 2;
            string line = MultiplyString("═", max);
            string message = $" {text} ";

            UI.LogFull($"╔{line}╗", color);
            UI.LogFull($"║{message}║", color);
            UI.LogFull($"╚{line}╝", color);

            if (tailingLine)
                UI.Log();
        }

        /// <summary>
        /// Renders a basic wrap around dialog.
        /// </summary>
        public static void RenderDialog(string text, ConsoleColor color = ConsoleColor.White)
            => RenderDialog(text, true, color);

        /// <summary>
        /// Renders an advanced dialog with a header and colored body lines.
        /// </summary>
        public static void RenderAdvancedColorDialog(string title, List<KeyValuePair<string, ConsoleColor>> lines, bool tailingLine = true, ConsoleColor color = ConsoleColor.White)
        {
            int max = title.Length;
            foreach (var l in lines)
                max = l.Key.Length < max ? max : l.Key.Length;

            max = max + 2;

            string line = MultiplyString("─", max);
            string titleLine = FillString($" {title}", " ", max);

            UI.LogFull($"┌{line}┐", color);
            UI.LogFull($"│{titleLine}│", color);
            UI.LogFull($"├{line}┤", color);

            foreach (var l in lines)
            {
                LogFullInLine($"│", color);
                LogFullInLine(FillString($" {l.Key}", " ", max), l.Value);
                LogFullInLine($"│\n", color);
            }

            UI.LogFull($"└{line}┘", color);

            if (tailingLine)
                UI.Log();
        }

        /// <summary>
        /// Renders an advanced dialog with a header and body.
        /// </summary>
        /// <param name="title">Header</param>
        /// <param name="text">Body</param>
        /// <param name="maxLineLength">Max length of body lines</param>
        /// <param name="tailingLine">Prints an empty line after the dialog if true</param>
        /// <param name="color">Color of the dialog</param>
        public static void RenderAdvancedDialog(string title, string text, int maxLineLength, bool tailingLine = true, ConsoleColor color = ConsoleColor.White)
        {
            maxLineLength = title.Length > maxLineLength ? title.Length : maxLineLength;
            List<string> lines = new();

            if (text.Length > maxLineLength)
            {
                var texts = text
                    .Replace("\t", " ")
                    .Split("\n")
                    .Select(t => Regex.Replace(t.Trim(), @"\s{2,}", " "))
                    .ToList();

                foreach (var t in texts)
                {
                    string x = t;

                    while (x.Length > maxLineLength)
                    {
                        int split = x.LastIndexOf(' ', Math.Min(maxLineLength, x.Length));

                        if (split <= 0)
                            split = maxLineLength;

                        string part = x.Substring(0, split).Trim();
                        lines.Add(part);

                        x = x.Substring(split).Trim();
                    }

                    if (!string.IsNullOrWhiteSpace(x))
                        lines.Add(x);
                }
            }
            else
                lines.Add(text);

            RenderAdvancedDialog(title, lines.ToArray(), tailingLine, color);
        }

        /// <summary>
        /// Renders an advanced dialog with a header and body.
        /// </summary>
        /// <param name="title">Header</param>
        /// <param name="lines">Body</param>
        /// <param name="tailingLine">Prints an empty line after the dialog if true</param>
        /// <param name="color">Color of the dialog</param>
        public static void RenderAdvancedDialog(string title, string[] lines, bool tailingLine = true, ConsoleColor color = ConsoleColor.White)
        {
            int max = title.Length;
            foreach (var l in lines)
                max = l.Length < max ? max : l.Length;

            max = max + 2;

            string line = MultiplyString("─", max);
            string titleLine = FillString($" {title}", " ", max);

            UI.LogFull($"┌{line}┐", color);
            UI.LogFull($"│{titleLine}│", color);
            UI.LogFull($"├{line}┤", color);

            foreach (var l in lines)
            {
                UI.LogFull($"│{FillString($" {l}", " ", max)}│", color);
            }

            UI.LogFull($"└{line}┘", color);

            if (tailingLine)
                UI.Log();
        }
        #endregion

        #region Helper Methods
        public static string MultiplyString(string s, int count)
        {
            string result = "";

            for (int i = 0; i < count; i++)
                result += s;

            return result;
        }

        public static string FillString(string s, string filler, int toLength)
        {
            string result = s;

            for (int i = 0; i < toLength - s.Length; i++)
                result += filler;

            return result;
        }

        public static string SizeString(string s, int size)
        {
            string result = string.Empty;

            if (s.Length > size)
                result = $"{s.Substring(0, size - 3)}...";
            else
                result = s;

            return result;
        }

        public static string Passwordify(string s)
        {
            string result = string.Empty;

            foreach (char c in s)
                result += "#";

            return result;
        }
        #endregion
    }
}
