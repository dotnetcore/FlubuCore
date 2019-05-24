using System;
using System.Xml;
using Flubu.VisualStudio.TaskRunner;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Flubu.VisualStudio.Base.TaskRunner
{
    internal class BindingsPersister
    {
        private const string BindingsName = "-vs-binding";
        private TaskRunnerProvider _provider;

        public BindingsPersister(TaskRunnerProvider provider)
        {
            _provider = provider;
        }

        public string Load(string configPath)
        {
            IVsTextView configTextView = TextViewUtil.FindTextViewFor(configPath);
            ITextUtil textUtil;

            if (configTextView != null)
            {
                textUtil = new VsTextViewTextUtil(configTextView);
            }
            else
            {
                textUtil = new FileTextUtil(configPath);
            }

            return textUtil.ReadAllText();
        }

        public bool Save(string configPath, string bindingsXml)
        {
            return true;
        }
    }

    public static class TextUtilExtensions2
    {
        public static void GetExtentInfo(this ITextUtil textUtil, int startIndex, int length, out int startLine, out int startLineOffset, out int endLine, out int endLineOffset)
        {
            textUtil.Reset();
            int lineNumber = 0, charCount = 0, lineCharCount = 0;
            string line;
            while (textUtil.TryReadLine(out line) && charCount < startIndex)
            {
                ++lineNumber;
                charCount += line.Length;
                lineCharCount = line.Length;
            }

            //We passed the line we want to be on, so back up
            int positionAtEndOfPreviousLine = charCount - lineCharCount;
            startLine = lineNumber - 1;
            startLineOffset = startIndex - positionAtEndOfPreviousLine;


            while (textUtil.TryReadLine(out line) && charCount < startIndex + length)
            {
                ++lineNumber;
                charCount += line.Length;
                lineCharCount = line.Length;
            }

            if (line != null)
            {
                positionAtEndOfPreviousLine = charCount - lineCharCount;
                endLineOffset = startIndex + length - positionAtEndOfPreviousLine;
            }
            else
            {
                endLineOffset = lineCharCount;
            }

            endLine = lineNumber - 1;
        }
    }
}