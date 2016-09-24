using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Packaging;

namespace Flubu.Tasks.Text
{
    public class UpdateJsonFileTask : TaskBase
    {
        private readonly string _fileName;
        private readonly Dictionary<string, JValue> _updates = new Dictionary<string, JValue>();
        private string _output;

        public UpdateJsonFileTask(string fileName)
        {
            _fileName = fileName;
        }

        public override string Description => $"Update JSON file {_fileName} to file {_output ?? _fileName}";

        public UpdateJsonFileTask Output(string fullFilePath)
        {
            _output = fullFilePath;
            return this;
        }

        public UpdateJsonFileTask Update(string path, JValue value)
        {
            _updates.Add(path, value);
            return this;
        }

        public UpdateJsonFileTask Update(params KeyValuePair<string, JValue>[] args)
        {
            _updates.AddRange(args);
            return this;
        }

        protected override void DoExecute(ITaskContext context)
        {
            if (!File.Exists(_fileName))
            {
                context.Fail($"JSON file {_fileName} not found!", 1);
                return;
            }

            if (_updates.Count <= 0)
            {
                context.Fail($"Nothing to update in file {_fileName}!", 2);
                return;
            }

            string file = File.ReadAllText(_fileName);
            JObject json = JObject.Parse(file);

            foreach (KeyValuePair<string, JValue> pair in _updates)
            {
                JToken token = json.SelectToken(pair.Key, false);

                if (token == null)
                {
                    context.Fail($"Propety {pair.Key} not found in {_fileName}", 3);
                    break;
                }

                token.Replace(pair.Value);
            }

            if (string.IsNullOrEmpty(_output))
            {
                _output = _fileName;
            }

            File.WriteAllText(_output, json.ToString(Formatting.Indented), Encoding.UTF8);
        }
    }
}
