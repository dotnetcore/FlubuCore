using System;
using System.Globalization;
using System.IO;

namespace FlubuCore.Tasks.Solution.VSSolutionBrowsing
{
    public class VSSolutionFileParser
    {
        private readonly TextReader _reader;

        public VSSolutionFileParser(TextReader reader)
        {
            _reader = reader;
        }

        public int LineCount { get; private set; }

        public string NextLine()
        {
            string line = null;

            do
            {
                if (_reader.Peek() == -1)
                    break;

                line = _reader.ReadLine();
                IncrementLineCount();

                ////if (log.IsDebugEnabled)
                ////    log.DebugFormat ("Read line ({0}): {1}", parserContext.LineCount, line);
            }
            while (line.Trim().Length == 0 || line.StartsWith("#", StringComparison.OrdinalIgnoreCase));

            return line;
        }

        public void ThrowParserException(string reason)
        {
            throw new ArgumentException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} (line {1})",
                    reason,
                    LineCount));
        }

        private void IncrementLineCount()
        {
            LineCount++;
        }
    }
}