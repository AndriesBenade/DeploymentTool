using Markdig.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Interfaces
{
    public interface ILogger
    {
        void Log(string context, string message);
        //string[] RenderLogs();
    }
}
