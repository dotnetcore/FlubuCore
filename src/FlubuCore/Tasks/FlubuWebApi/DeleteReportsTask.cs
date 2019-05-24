using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public class DeleteReportsTask : WebApiBaseTask<DeleteReportsTask, int>
    {
        private string _subDirectory = null;
        private string _description;

        public DeleteReportsTask(IWebApiClientFactory webApiClient)
            : base(webApiClient)
        {
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return
                        $"Deletes all genereated reports in 'Reports' directory from flubu server.";
                }

                return _description;
            }

            set => _description = value;
        }

        public DeleteReportsTask DeleteFromSubDirectory(string subDirectory)
        {
            _subDirectory = subDirectory;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            var task = DoExecuteAsync(context);

            return task.GetAwaiter().GetResult();
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            var request = new CleanReportsDirectoryRequest
            {
                SubDirectoryToDelete = _subDirectory
            };

            var client = WebApiClientFactory.Create(context.Properties.Get<string>(BuildProps.LastWebApiBaseUrl));
            var response = await client.ExecuteAsync(c => c.CleanReportsDirectoryAsync(request));

            if (response != null)
            {
                throw new TaskExecutionException($"Upload script failed: ErrorCode: {response.ErrorCode} ErrorMessage: {response.ErrorMessage}", 99);
            }

            return 0;
        }
    }
}