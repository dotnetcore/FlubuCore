using System;

using Flubu.IO;

namespace Flubu.Packaging
{
    public interface ICopier
    {
        void Copy(FileFullPath sourceFileName, FileFullPath destinationFileName);
    }
}