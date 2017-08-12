using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.FlubuWebApi;

namespace FlubuCore.Context.FluentInterface
{
	public class WebApiFluentInterface : IWebApiFluentInterface
	{
		public TaskContext Context { get; set; }

		public UploadPackageTask UploadPackageTask(string directoryPath, string packageSearchPattern = null)
		{
			return Context.CreateTask<UploadPackageTask>(directoryPath).PackageSearchPattern(packageSearchPattern);
		}

		public ExecuteFlubuScriptTask ExecuteScriptTask(string mainCommand, string scriptFilePath)
		{
			return Context.CreateTask<ExecuteFlubuScriptTask>(mainCommand, scriptFilePath);
		}
	}
}
