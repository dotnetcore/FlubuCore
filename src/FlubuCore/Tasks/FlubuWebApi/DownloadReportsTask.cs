using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public class DownloadReportsTask : WebApiBaseTask<DownloadReportsTask, int>
    {
        private readonly string _saveAs;
        private bool _failWhenNoReportsFound;
        private string _subDirectory = null;
        private string _description;

        public DownloadReportsTask(IWebApiClientFactory webApiClient, string saveAs)
            : base(webApiClient)
        {
            _saveAs = saveAs;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Downloads reports from flubu server.";
                }

                return _description;
            }

            set { _description = value; }
        }

        public DownloadReportsTask DownloadFromSubDirectory(string subDirectory)
        {
            _subDirectory = subDirectory;
            return this;
        }

        public DownloadReportsTask FailWhenNoReportsFound()
        {
            _failWhenNoReportsFound = true;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            Task<int> task = DoExecuteAsync(context);

            return task.GetAwaiter().GetResult();
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            if (string.IsNullOrEmpty(_saveAs))
            {
                throw new ArgumentNullException(nameof(_saveAs));
            }

            var extension = Path.GetExtension(_saveAs);

            if (string.IsNullOrEmpty(extension) || !extension.Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                throw new TaskExecutionException("SaveAs file extension must be .zip", 99);
            }

            var client = WebApiClientFactory.Create(context.Properties.Get<string>(BuildProps.LastWebApiBaseUrl));

            try
            {
                var reports = await client.DownloadReportsAsync(new DownloadReportsRequest()
                {
                    DownloadFromSubDirectory = _subDirectory
                }) as MemoryStream;

                using (FileStream file = new FileStream(_saveAs, FileMode.Create, FileAccess.Write))
                {
                    reports.WriteTo(file);
                    reports.Close();
                    file.Close();
                }
            }
            catch (WebApiException e)
            {
                if (e.ErrorCode == "NoReportsFound" && !_failWhenNoReportsFound)
                {
                    context.LogInfo("No reports found on server.");
                    return 0;
                }

                throw new TaskExecutionException($"Download reports failed: ErrorCode: {e.ErrorCode} ErrorMessage: {e.ErrorMessage}", 99);
            }

            return 0;
        }
    }
}
