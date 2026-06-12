using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Models.SettingsModules
{
    public class LoggerSettings
    {
        public bool Enabled { get; set; }
        public LoggerMode Mode { get; set; }
        public FileLoggerSettings File { get; set; }
        public SqlLoggerSettings Sql { get; set; }
    }

    public class FileLoggerSettings
    {
        public string FilenameFormat { get; set; }
        public string LogPath { get; set; }
    }

    public class SqlLoggerSettings
    {
        public string ConnectionString { get; set; }
    }

    public enum LoggerMode
    {
        File = 0,
        Sql = 1
    }
}
