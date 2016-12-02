using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Text
{
    public class ReplaceTextTask : TaskBase<int>
    {
        private readonly Dictionary<string, string> _tokens = new Dictionary<string, string>();

        private readonly string _sourceFileName;

        private string _destinationFileName;

        public ReplaceTextTask(string sourceFileName)
        {
            _sourceFileName = sourceFileName;
            _destinationFileName = sourceFileName;
        }

        public Encoding SourceFileEncoding { get; set; } = Encoding.UTF8;

        public Encoding DestionationFileEncoding { get; set; } = Encoding.UTF8;

        public ReplaceTextTask ToDestination(string file)
        {
            _destinationFileName = file;
            return this;
        }

        public ReplaceTextTask Replace(string oldValue, string newValue)
        {
            _tokens.Add(oldValue, newValue);
            return this;
        }

        public ReplaceTextTask Replace(params Tuple<string, string>[] tokens)
        {
            foreach (Tuple<string, string> token in tokens)
            {
                _tokens.Add(token.Item1, token.Item2);
            }

            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            context.LogInfo($"Replacing text in file {_sourceFileName}");

            string tokenizedContent = File.ReadAllText(_sourceFileName, SourceFileEncoding);

            string finalContent = ReplaceTokens(tokenizedContent);

            File.WriteAllText(_destinationFileName, finalContent, DestionationFileEncoding);

            return 0;
        }

        private string ReplaceTokens(string tokenizedContent)
        {
            foreach (KeyValuePair<string, string> entry in _tokens)
                tokenizedContent = tokenizedContent.Replace(entry.Key, entry.Value);

            return tokenizedContent;
        }
    }
}
