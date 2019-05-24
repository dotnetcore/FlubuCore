using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public class DeletePackagesTask : WebApiBaseTask<DeletePackagesTask, int>
    {
        private string _subDirectory = null;
        private string _description;

        public DeletePackagesTask(IWebApiClientFactory webApiClient)
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
                        $"Deletes all previously uploaded packages from flubu server.";
                }

                return _description;
            }

            set => _description = value;
        }

        public DeletePackagesTask DeleteFromSubDirectory(string subDirectory)
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
            var request = new CleanPackagesDirectoryRequest
            {
                SubDirectoryToDelete = _subDirectory
            };

            var client = WebApiClientFactory.Create(context.Properties.Get<string>(BuildProps.LastWebApiBaseUrl));
            var response = await client.ExecuteAsync(c => c.DeletePackagesAsync(request));

            if (response != null)
            {
                throw new TaskExecutionException($"Delete packages failed: ErrorCode: {response.ErrorCode} ErrorMessage: {response.ErrorMessage}", 99);
            }

            return 0;
        }
    }
}