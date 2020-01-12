using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.WebApi.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlubuCore.Tasks.Text
{
    public class UpdateJsonFileTask : TaskBase<int, UpdateJsonFileTask>
    {
        private readonly string _fileName;
        private readonly Dictionary<string, JValue> _updates = new Dictionary<string, JValue>();
        private string _output;
        private bool _failIfNotFound = true;
        private bool _failOnTypeMismatch;
        private string _description;

        public UpdateJsonFileTask(string fileName)
        {
            _fileName = fileName;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Updates json file '{_fileName}'";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// Set the fileName of the new json file. If not set same file is updated.
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns></returns>
        public UpdateJsonFileTask Output(string fullFilePath)
        {
            _output = fullFilePath;
            return this;
        }

        /// <summary>
        ///  Updates json property/element with specified value,
        /// </summary>
        /// <param name="path">Json path to the element to be updated</param>
        /// <param name="value">New value of the json element</param>
        /// <returns></returns>
        public UpdateJsonFileTask Update(string path, string value)
        {
            _updates.Add(path, new JValue(value));
            return this;
        }

        /// <summary>
        ///  Updates json property/element with specified value,
        /// </summary>
        /// <param name="path">Json path to the element to be updated</param>
        /// <param name="value">New value of the json element</param>
        /// <returns></returns>
        public UpdateJsonFileTask Update(string path, int value)
        {
            _updates.Add(path, new JValue(value));
            return this;
        }

        /// <summary>
        ///  Updates json property/element with specified value,
        /// </summary>
        /// <param name="path">Json path to the element to be updated</param>
        /// <param name="value">New value of the json element</param>
        /// <returns></returns>
        public UpdateJsonFileTask Update(string path, long value)
        {
            _updates.Add(path, new JValue(value));
            return this;
        }

        /// <summary>
        ///  Updates json property/element with specified value,
        /// </summary>
        /// <param name="path">Json path to the element to be updated</param>
        /// <param name="value">New value of the json element</param>
        /// <returns></returns>
        public UpdateJsonFileTask Update(string path, double value)
        {
            _updates.Add(path, new JValue(value));
            return this;
        }

        /// <summary>
        ///  Updates json property/element with specified value,
        /// </summary>
        /// <param name="path">Json path to the element to be updated</param>
        /// <param name="value">New value of the json element</param>
        /// <returns></returns>
        public UpdateJsonFileTask Update(string path, decimal value)
        {
            _updates.Add(path, new JValue(value));
            return this;
        }

        /// <summary>
        ///  Updates json property/element with specified value,
        /// </summary>
        /// <param name="path">Json path to the element to be updated</param>
        /// <param name="value">New value of the json element</param>
        /// <returns></returns>
        public UpdateJsonFileTask Update(string path, DateTime value)
        {
            _updates.Add(path, new JValue(value));
            return this;
        }

        /// <summary>
        ///  Updates json property/element with specified value,
        /// </summary>
        /// <param name="path">Json path to the element to be updated</param>
        /// <param name="value">New value of the json element</param>
        /// <returns></returns>
        public UpdateJsonFileTask Update(params KeyValuePair<string, JValue>[] args)
        {
            _updates.AddRange(args);
            return this;
        }

        /// <summary>
        /// If <c>true</c> task fails with exception if any of the properties to be updated are not found.
        /// </summary>
        /// <param name="fail"></param>
        /// <returns></returns>
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

        protected override int DoExecute(ITaskContextInternal context)
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

            DoLogInfo($"Update JSON file {_fileName} to file {_output ?? _fileName}. With {_updates.Count} updates");
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
                        DoLogInfo($"Propety {pair.Key} not found in {_fileName}");

                    res = 3;
                    continue;
                }

                if (token.Type != pair.Value.Type)
                {
                    if (_failOnTypeMismatch)
                    {
                        context.Fail($"Propety {pair.Key} type mismatch.", 4);
                        continue;
                    }

                    DoLogInfo($"Propety {pair.Key} type mismatch.");

                    res = 4;
                }

                DoLogInfo($"Replacing {token.Path} with {pair.Value}.");
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
