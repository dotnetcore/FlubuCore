using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.NetCore;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface IToolsFluentInterface
    {
        DotnetToolInstall Install(string nugetPackageId);
    }
}
