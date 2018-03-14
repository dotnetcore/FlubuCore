using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public class DeletePackagesTask : WebApiBaseTask<DeletePackagesTask, int>
    {
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

        protected override int DoExecute(ITaskContextInternal context)
        {
            var task = DoExecuteAsync(context);

            return task.GetAwaiter().GetResult();
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            var client = WebApiClientFactory.Create(context.Properties.Get<string>(BuildProps.LastWebApiBaseUrl));
            await client.DeletePackagesAsync(new CleanPackagesDirectoryRequest());
            return 0;
        }
    }
}