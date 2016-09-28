using System;

namespace Flubu.Packaging
{
    public interface IFileFilter
    {
        bool IsPassedThrough(string fileName);
    }
}