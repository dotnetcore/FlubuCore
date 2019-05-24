using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Flubu.VisualStudio.Base.TaskRunner
{
    public static class TextViewUtil
    {
        public static IVsTextView FindTextViewFor(string filePath)
        {
            IVsWindowFrame frame = FindWindowFrame(filePath);
            if (frame != null)
            {
                IVsTextView textView;

                if (GetTextViewFromFrame(frame, out textView))
                {
                    return textView;
                }
            }

            return null;
        }

        private static IEnumerable<IVsWindowFrame> EnumerateDocumentWindowFrames()
        {
            IVsUIShell shell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;

            if (shell != null)
            {
                IEnumWindowFrames framesEnum;

                int hr = shell.GetDocumentWindowEnum(out framesEnum);

                if (hr == VSConstants.S_OK && framesEnum != null)
                {
                    IVsWindowFrame[] frames = new IVsWindowFrame[1];
                    uint fetched;

                    while (framesEnum.Next(1, frames, out fetched) == VSConstants.S_OK && fetched == 1)
                    {
                        yield return frames[0];
                    }
                }
            }
        }

        private static IVsWindowFrame FindWindowFrame(string filePath)
        {
            foreach (IVsWindowFrame currentFrame in EnumerateDocumentWindowFrames())
            {
                if (IsFrameForFilePath(currentFrame, filePath))
                {
                    return currentFrame;
                }
            }

            return null;
        }

        private static bool GetPhysicalPathFromFrame(IVsWindowFrame frame, out string frameFilePath)
        {
            object propertyValue;

            int hr = frame.GetProperty((int)__VSFPROPID.VSFPROPID_pszMkDocument, out propertyValue);
            if (hr == VSConstants.S_OK && propertyValue != null)
            {
                frameFilePath = propertyValue.ToString();
                return true;
            }

            frameFilePath = null;
            return false;
        }

        private static bool GetTextViewFromFrame(IVsWindowFrame frame, out IVsTextView textView)
        {
            textView = VsShellUtilities.GetTextView(frame);

            return textView != null;
        }

        private static bool IsFrameForFilePath(IVsWindowFrame frame, string filePath)
        {
            string frameFilePath;

            if (GetPhysicalPathFromFrame(frame, out frameFilePath))
            {
                return String.Equals(filePath, frameFilePath, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
