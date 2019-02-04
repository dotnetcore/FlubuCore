using System;
using System.Collections.Generic;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Text
{
    public class T4TemplateTask : ExternalProcessTaskBase<int, T4TemplateTask>
    {
        private const string ExecutableName = "TextTransform.exe";
        private readonly string _templateFileName;

        private readonly List<string> _executablePaths = new List<string>
        {
            "c:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\BuildTools\\Common7\\IDE",
            "c:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Professional\\Common7\\IDE",
            "c:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Enterprise\\Common7\\IDE",
            "c:\\Program Files (x86)\\Common Files\\Microsoft Shared\\TextTemplating\\14.0"
        };

        public T4TemplateTask(string templateFileName)
        {
            _templateFileName = templateFileName;
        }

        protected override string Description
        {
            get => "Generate T4 text template.";
            set { }
        }

        public T4TemplateTask AddAdditionalPath(string path)
        {
            _executablePaths.Add(path);
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            ExecutablePath = FindExecutable();

            if (string.IsNullOrEmpty(ExecutablePath))
                throw new ArgumentException("T4 TextTransform utility not found!");

            InsertArgument(0, _templateFileName);

            return base.DoExecute(context);
        }

        private string FindExecutable()
        {
            foreach (var path in _executablePaths)
            {
                if (File.Exists(path))
                    return path;

                var combined = Path.Combine(path, ExecutableName);
                if (File.Exists(combined))
                    return combined;
            }

            return null;
        }
    }
}