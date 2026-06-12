using DeploymentTool.Base;
using DeploymentTool.Models;
using Markdig.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Logging
{
    public class FileLogger : Logger
    {
        private string _file;
        private string _path => _settings.Deployment.Logging.File.LogPath;

        #region Constructor
        public FileLogger(Settings settings) : base(settings)
        {
            Initialize();
        }
        #endregion

        protected override void PersistLog(string context, string message)
            => WriteLog($"[{DateTime.Now}] [{context}] {message}");

        #region Private Methods
        private void Initialize()
        {
            _file = Path.Combine(_path, ResolveFilename());

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            CreateLog();
        }

        private void CreateLog()
            => File.WriteAllText(_file, $"Log created on {DateTime.Now}\n\n");

        private void WriteLog(string log)
            => File.AppendAllText(_file, $"{log}\n");

        private string ResolveFilename()
        {
            string file = _settings.Deployment.Logging.File.FilenameFormat;

            file = file.Replace("<pipeline>", _settings.Pipeline.Name);

            if (file.Contains("[") && file.Contains("]"))
            {
                string dateFormat = file.Substring(file.IndexOf("[") + 1, file.IndexOf("]") - file.IndexOf("[") - 1);
                string formattedDate = DateTime.Now.ToString(dateFormat);

                file = file.Replace($"[{dateFormat}]", formattedDate);
            }


            return file;
        }
        #endregion
    }
}
