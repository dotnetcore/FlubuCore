using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Text
{
    /// <inheritdoc />
    public class ReplaceTokensTask : TaskBase<int, ReplaceTokensTask>
    {
        private readonly Dictionary<string, string> _tokens = new Dictionary<string, string>();

        private readonly string _sourceFileName;

        private string _destinationFileName;
        private string _token;
        private Encoding _destinationEncoding = Encoding.UTF8;
        private Encoding _sourceEncoding = Encoding.UTF8;
        private bool _useTmpFile;
        private string _description;

        /// <inheritdoc />
        public ReplaceTokensTask(string sourceFileName)
        {
            _sourceFileName = sourceFileName;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Replaces tokens in file '{_sourceFileName}' to file '{_destinationFileName}";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// Sets the encoding of the source file. Default is UTF8
        /// </summary>
        public ReplaceTokensTask SourceFileEncoding(Encoding encoding)
        {
            _sourceEncoding = encoding;
            return this;
        }

        /// <summary>
        /// Sets the encoding of the destination file. Default is UTF8.
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public ReplaceTokensTask DestionationFileEncoding(Encoding encoding)
        {
            _destinationEncoding = encoding;
            return this;
        }

        /// <summary>
        /// Use token prefix and suffix for key. {token}{key}{token} will be replaced with key's value.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ReplaceTokensTask UseToken(string token)
        {
            _token = token;
            return this;
        }

        /// <summary>
        /// Sets the destination filename. If not specified source file will be replaced.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public ReplaceTokensTask ToDestination(string file)
        {
            _destinationFileName = file;
            return this;
        }

        /// <summary>
        /// Replace old value with the new one.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public ReplaceTokensTask Replace(string oldValue, string newValue)
        {
            _tokens.Add(oldValue, newValue);
            return this;
        }

        /// <summary>
        /// Replace old value with the new one.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public ReplaceTokensTask Replace(params Tuple<string, string>[] tokens)
        {
            foreach (Tuple<string, string> token in tokens)
            {
                _tokens.Add(token.Item1, token.Item2);
            }

            return this;
        }

        /// <summary>
        /// Create new file with the source file name and appended with .tmp.
        /// </summary>
        /// <returns></returns>
        public ReplaceTokensTask UseTmpFile()
        {
            _useTmpFile = true;
            return this;
        }

        /// <inheritdoc />
        protected override int DoExecute(ITaskContextInternal context)
        {
            DoLogInfo($"Replacing text in file {_sourceFileName}");

            string tokenizedContent = File.ReadAllText(_sourceFileName, _sourceEncoding);

            string finalContent = ReplaceTokens(tokenizedContent);

            if (_useTmpFile)
                _destinationFileName = $"{_sourceFileName}.tmp";
            else if (string.IsNullOrEmpty(_destinationFileName))
                _destinationFileName = _sourceFileName;

            File.WriteAllText(_destinationFileName, finalContent, _destinationEncoding);

            return 0;
        }

        private string ReplaceTokens(string tokenizedContent)
        {
            foreach (KeyValuePair<string, string> entry in _tokens)
            {
                string key = string.IsNullOrEmpty(_token) ? entry.Key : $"{_token}{entry.Key}{_token}";

                tokenizedContent = tokenizedContent.Replace(key, entry.Value);
            }

            return tokenizedContent;
        }
    }
}
