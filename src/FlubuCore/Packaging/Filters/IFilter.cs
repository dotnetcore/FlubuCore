using System;

namespace FlubuCore.Packaging
{
    public interface IFilter
    {
        bool IsPassedThrough(string path);
    }
}