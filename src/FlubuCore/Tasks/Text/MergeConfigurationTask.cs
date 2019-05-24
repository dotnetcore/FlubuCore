using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.Context;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlubuCore.Tasks.Text
{
    public class MergeConfigurationTask : TaskBase<int, MergeConfigurationTask>
    {
        private readonly string _fileName;
        private readonly List<string> _sourceFiles = new List<string>();

        public MergeConfigurationTask(string outFile, List<string> sourceFiles)
        {
            _fileName = outFile;
            _sourceFiles.AddRange(sourceFiles);
        }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
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
