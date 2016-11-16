using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Text
{
    public class ReplaceTokensTask : TaskBase<int>
    {
        private readonly string _destinationFileName;

        private readonly Dictionary<string, string> _tokens = new Dictionary<string, string>();

        private readonly string _sourceFileName;

        private Encoding _sourceFileEncoding = Encoding.UTF8;

        private Encoding _destionationFileEncoding = Encoding.UTF8;

        public ReplaceTokensTask(
            string sourceFileName,
            string destinationFileName)
        {
            _sourceFileName = sourceFileName;
            _destinationFileName = destinationFileName;
        }

        /// <summary>
        /// Gets the task description.
        /// </summary>
        /// <value>The task description.</value>
        public string Description
        {
            get
            {
                return string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "Replace tokens in file '{0}' to file '{1}'",
                    _sourceFileName,
                    _destinationFileName);
            }
        }

        public Encoding SourceFileEncoding
        {
            get { return _sourceFileEncoding; }
            set { _sourceFileEncoding = value; }
        }

        public Encoding DestionationFileEncoding
        {
            get { return _destionationFileEncoding; }
            set { _destionationFileEncoding = value; }
        }

        public void AddTokenValue(string token, string value)
        {
            _tokens.Add(token, value);
        }

        protected override int DoExecute(ITaskContext context)
        {
            string tokenizedContent = File.ReadAllText(_sourceFileName, _sourceFileEncoding);

            string finalContent = ReplaceTokens(tokenizedContent);

            File.WriteAllText(_destinationFileName, finalContent, _destionationFileEncoding);

            return 0;
        }

        private string ReplaceTokens(string tokenizedContent)
        {
            foreach (KeyValuePair<string, string> entry in _tokens)
                tokenizedContent = tokenizedContent.Replace("$" + entry.Key + "$", entry.Value);

            return tokenizedContent;
        }
    }
}
