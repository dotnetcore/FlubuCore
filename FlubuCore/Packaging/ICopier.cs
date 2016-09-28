using System;
using FlubuCore.IO;

namespace FlubuCore.Packaging
{
    public interface ICopier
    {
        void Copy(FileFullPath sourceFileName, FileFullPath destinationFileName);
    }
}