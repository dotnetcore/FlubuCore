using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.Context;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlubuCore.Tasks.Text
{
    public class MergeConfigurationTask : TaskBase
    {
        private readonly string _fileName;
        private readonly List<string> _sourceFiles = new List<string>();

        public MergeConfigurationTask(string outFile)
        {
            _fileName = outFile;
        }

        public MergeConfigurationTask(string outFile, params string[] sourceFiles)
        {
            _fileName = outFile;
            _sourceFiles.AddRange(sourceFiles);
        }

        protected override int DoExecute(ITaskContext context)
        {
            JObject dest = new JObject();

            foreach (string file in _sourceFiles)
            {
                JObject src = JObject.Parse(File.ReadAllText(file));

                foreach (JToken child in src.Children())
                {
                    dest.Add(child);
                }
            }

            File.WriteAllText(_fileName, dest.ToString(Formatting.Indented), Encoding.UTF8);

            return 0;
        }
    }
}
