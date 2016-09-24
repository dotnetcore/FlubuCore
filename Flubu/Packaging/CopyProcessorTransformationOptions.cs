using System;

namespace Flubu.Packaging
{
    [Flags]
    public enum CopyProcessorTransformationOptions
    {
        None = 0,
        SingleFile = 1 << 0,
        FlattenDirStructure = 1 << 1,
    }
}