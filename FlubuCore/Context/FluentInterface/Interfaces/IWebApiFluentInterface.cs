using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.FlubuWebApi;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface IWebApiFluentInterface
    {
	    UploadPackageTask UploadPackageTask(string directoryPath, string packageSearchPattern);

	    ExecuteFlubuScriptTask ExecuteScriptTask(string mainCommand, string scriptFilePath);
    }
}
