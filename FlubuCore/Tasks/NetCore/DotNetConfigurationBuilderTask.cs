using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.Context;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetConfigurationBuilderTask : TaskBase<int, DotnetConfigurationBuilderTask>
    {
        private readonly string _destinationConfiguration;
        private readonly List<string> _sourceFiles = new List<string>();

        public DotnetConfigurationBuilderTask(string destinationConfigurationFile, string sourceConfigFile)
        {
            _destinationConfiguration = destinationConfigurationFile;
            _sourceFiles.Add(sourceConfigFile);
        }

        public DotnetConfigurationBuilderTask(string destinationConfigurationFile, params string[] sourceConfigFiles)
        {
            _destinationConfiguration = destinationConfigurationFile;
            _sourceFiles.AddRange(sourceConfigFiles);
        }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < _sourceFiles.Count; i++)
            {
                string file = _sourceFiles[i];
                string current = File.ReadAllText(file);
            }

            File.WriteAllText(_destinationConfiguration, builder.ToString(), Encoding.UTF8);
            return 0;
        }
    }
}
