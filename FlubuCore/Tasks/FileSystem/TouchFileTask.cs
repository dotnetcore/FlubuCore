using System;
using System.IO;
using FlubuCore.Context;

namespace FlubuCore.Tasks.FileSystem
{
    public class TouchFileTask : TaskBase<int, TouchFileTask>
    {
        private readonly string _fileName;

        public TouchFileTask(string fileName)
        {
            _fileName = fileName;
        }

        protected override string Description
        {
            get => $"Touch file {_fileName}";
            set { }
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            DoLogInfo($"Touch file '{_fileName}'");

            File.SetLastWriteTimeUtc(_fileName, DateTime.UtcNow);

            return 0;
        }
    }
}