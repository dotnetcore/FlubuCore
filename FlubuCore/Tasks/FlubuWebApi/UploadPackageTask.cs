using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public class UploadPackageTask : WebApiBaseTask<UploadPackageTask, int>
    {
        private string _packageSearchPattern;

        private string _directoryPath;

        private string _uploadToSubDirectory;

        private string _description;

        public UploadPackageTask(IWebApiClientFactory client, string directoryPath)
            : base(client)
        {
            _directoryPath = directoryPath;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Uploads package found in directory {_directoryPath} to flubu server.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// The search string to match against the names of files(packages). This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.
        /// </summary>
        /// <param name="packageSearchPattern"></param>
        /// <returns></returns>
        public UploadPackageTask PackageSearchPattern(string packageSearchPattern)
        {
            _packageSearchPattern = packageSearchPattern;
            return this;
        }

        public UploadPackageTask UploadToSubDirectory(string subDirectory)
        {
            _uploadToSubDirectory = subDirectory;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            Task<int> task = DoExecuteAsync(context);

            return task.GetAwaiter().GetResult();
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            var client = WebApiClientFactory.Create(context.Properties.Get<string>(BuildProps.LastWebApiBaseUrl));
            var response = await client.ExecuteAsync(c => c.UploadPackageAsync(new UploadPackageRequest
            {
                PackageSearchPattern = _packageSearchPattern,
                DirectoryPath = _directoryPath,
                UploadToSubDirectory = _uploadToSubDirectory,
            }));

            if (response.Error != null)
            {
                throw new TaskExecutionException($"Upload packages failed: ErrorCode: {response.Error.ErrorCode} ErrorMessage: {response.Error.ErrorMessage}", 99);
            }

            if (response.Data == null || response.Data.Count == 0)
            {
                context.LogInfo("No packages uploaded.");
            }
            else
            {
                foreach (var package in response.Data)
                {
                    context.LogInfo($"Uploaded: {package}");
                }
            }

            return 0;
        }
    }
}
