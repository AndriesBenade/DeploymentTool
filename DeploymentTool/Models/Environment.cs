using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentTool.Models
{
    /// <summary>
    /// Collection of deployment pipelines for environmental deployments.
    /// </summary>
    public class Environment
    {
        private string _name;

        public List<Settings> Pipelines { get; set; }

        #region Public Methods
        public void SetName(string name)
            => _name = name;

        public string GetName()
            => _name;

        public void Initialize(string filename)
        {
            SetName(Path.GetFileNameWithoutExtension(filename));

            for (int i = 0; i < Pipelines.Count; i++)
            {
                Pipelines[i].Initialize(filename);

                for (int ii = 0; ii < Pipelines.Count; ii++)
                {
                    if (i == ii) continue;

                    if (Pipelines[i].Pipeline.Name.ToLower() == Pipelines[ii].Pipeline.Name.ToLower())
                        throw new Exception($"Duplicate pipeline name detected in environment: {Pipelines[i].Pipeline.Name}");
                }
            }
        }
        #endregion
    }
}
