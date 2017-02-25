using System;

namespace FlubuCore.Packaging
{
    public interface IFileFilter
    {
        bool IsPassedThrough(string fileName);
    }
}