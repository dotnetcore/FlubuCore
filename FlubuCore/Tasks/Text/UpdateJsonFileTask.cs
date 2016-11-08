using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.Context;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Packaging;

namespace FlubuCore.Tasks.Text
{
    public class UpdateJsonFileTask : TaskBase
    {
        private readonly string _fileName;
        private readonly Dictionary<string, JValue> _updates = new Dictionary<string, JValue>();
        private string _output;
        private bool _failIfNotFound = true;
        private bool _failOnTypeMismatch;

        public UpdateJsonFileTask(string fileName)
        {
            _fileName = fileName;
        }

        public UpdateJsonFileTask Output(string fullFilePath)
        {
            _output = fullFilePath;
            return this;
        }

        public UpdateJsonFileTask Update(string path, string value)
        {
            _updates.Add(path, new JValue(value));
            return this;
        }

        public UpdateJsonFileTask Update(string path, int value)
        {
            _updates.Add(path, new JValue(value));
            return this;
        }

        public UpdateJsonFileTask Update(string path, long value)
        {
            _updates.Add(path, new JValue(value));
            return this;
        }

        public UpdateJsonFileTask Update(string path, double value)
        {
            _updates.Add(path, new JValue(value));
            return this;
        }

        public UpdateJsonFileTask Update(string path, decimal value)
        {
            _updates.Add(path, new JValue(value));
            return this;
        }

        public UpdateJsonFileTask Update(string path, DateTime value)
        {
            _updates.Add(path, new JValue(value));
            return this;
        }

        public UpdateJsonFileTask Update(params KeyValuePair<string, JValue>[] args)
        {
            _updates.AddRange(args);
            return this;
        }

        public UpdateJsonFileTask FailIfPropertyNotFound(bool fail)
        {
            _failIfNotFound = fail;
            return this;
        }

        public UpdateJsonFileTask FailOnTypeMismatch(bool fail)
        {
            _failOnTypeMismatch = fail;
            return this;
        }

        protected override int DoExecute(ITaskContext context)
        {
            if (!File.Exists(_fileName))
            {
                context.Fail($"JSON file {_fileName} not found!", 1);
                return 1;
            }

            if (_updates.Count <= 0)
            {
                context.Fail($"Nothing to update in file {_fileName}!", 2);
                return 2;
            }

            context.LogInfo($"Update JSON file {_fileName} to file {_output ?? _fileName}");
            string file = File.ReadAllText(_fileName);
            JObject json = JObject.Parse(file);
            int res = 0;
            foreach (KeyValuePair<string, JValue> pair in _updates)
            {
                JToken token = json.SelectToken(pair.Key, false);

                if (token == null)
                {
                    if (_failIfNotFound)
                        context.Fail($"Propety {pair.Key} not found in {_fileName}", 3);
                    else
                        context.LogInfo($"Propety {pair.Key} not found in {_fileName}");

                    res = 3;
                    break;
                }

                if (token.Type != pair.Value.Type)
                {
                    if (_failOnTypeMismatch)
                    {
                        context.Fail($"Propety {pair.Key} type mismatch.", 4);
                        break;
                    }

                    context.LogInfo($"Propety {pair.Key} type mismatch.");

                    res = 4;
                }

                context.LogInfo($"Replacing {token.Path} with {pair.Value}.");
                token.Replace(pair.Value);
            }

            if (string.IsNullOrEmpty(_output))
            {
                _output = _fileName;
            }

            File.WriteAllText(_output, json.ToString(Formatting.Indented), Encoding.UTF8);
            return res;
        }
    }
}
