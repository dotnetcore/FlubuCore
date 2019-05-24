using System;
using System.IO;

namespace Flubu.VisualStudio.Base.TaskRunner
{
    internal class FileTextUtil : ITextUtil
    {
        private int _currentLineLength;
        private readonly string _filename;
        private int _lineNumber;

        public FileTextUtil(string filename)
        {
            _filename = filename;
        }

        public Range CurrentLineRange => new Range { LineNumber = _lineNumber, LineRange = new LineRange { Start = 0, Length = _currentLineLength } };

        public bool Delete(Range range)
        {
            if (range.LineRange.Length == 0)
            {
                return true;
            }

            string fileContents = File.ReadAllText(_filename);

            using (StringReader reader = new StringReader(fileContents))
            using (TextWriter writer = new StreamWriter(File.Open(_filename, FileMode.Create)))
            {
                int remainingCharacters = range.LineRange.Length;
                int currentStart = range.LineRange.Start;
                string lineText;
                while (remainingCharacters > 0 && SeekTo(reader, writer, range, out lineText))
                {
                    int trimFromLine = Math.Min(lineText.Length - currentStart, remainingCharacters);
                    writer.WriteLine(lineText.Substring(0, currentStart) + lineText.Substring(currentStart + trimFromLine));
                    remainingCharacters -= trimFromLine;
                    range.LineNumber = 0;
                    range.LineRange.Start = 0;
                    range.LineRange.Length = remainingCharacters;
                    currentStart = 0;
                }

                lineText = reader.ReadLine();

                while (lineText != null)
                {
                    writer.WriteLine(lineText);
                    lineText = reader.ReadLine();
                }
            }

            return true;
        }

        public bool Insert(Range range, string text, bool addNewline)
        {
            if (text.Length == 0)
            {
                return true;
            }

            string fileContents = File.ReadAllText(_filename);

            using (StringReader reader = new StringReader(fileContents))
            using (TextWriter writer = new StreamWriter(File.Open(_filename, FileMode.Create)))
            {
                string lineText;
                if (SeekTo(reader, writer, range, out lineText))
                {
                    writer.WriteLine(lineText.Substring(0, range.LineRange.Start) + text + (addNewline ? Environment.NewLine : string.Empty) + lineText.Substring(range.LineRange.Start));
                }

                lineText = reader.ReadLine();

                while (lineText != null)
                {
                    writer.WriteLine(lineText);
                    lineText = reader.ReadLine();
                }
            }

            return true;
        }

        public bool TryReadLine(out string line)
        {
            line = null;
            Stream stream = File.OpenRead(_filename);
            using (TextReader reader = new StreamReader(stream))
            {
                int lineCount = _lineNumber;
                for (int i = 0; i < lineCount + 1; ++i)
                {
                    line = reader.ReadLine();
                }

                if (line != null)
                {
                    _currentLineLength = line.Length;
                    ++_lineNumber;
                    return true;
                }

                _currentLineLength = 0;
                return false;
            }
        }

        public string ReadAllText()
        {
            return File.ReadAllText(_filename).Replace("\r", "").Replace("\n", "");
        }

        public void Reset()
        {
            _lineNumber = 0;
        }

        private bool SeekTo(StringReader reader, TextWriter writer, Range range, out string lineText)
        {
            bool success = true;

            for (int lineNumber = 0; lineNumber < range.LineNumber; ++lineNumber)
            {
                string line = reader.ReadLine();

                if (line != null)
                {
                    writer.WriteLine(line);
                }
                else
                {
                    success = false;
                    break;
                }
            }

            lineText = reader.ReadLine();

            if (success)
            {
                if (lineText != null)
                {
                    if (lineText.Length < range.LineRange.Start)
                    {
                        success = false;
                        writer.WriteLine(lineText);
                    }
                }
            }

            return success;
        }

        public void FormatRange(LineRange range)
        {
        }
    }
}
