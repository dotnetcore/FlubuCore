using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using Microsoft.Web.Administration;

namespace FlubuCore.Tasks.Iis
{
    public class DeleteAppPoolTask : TaskBase<int, IDeleteAppPoolTask>, IDeleteAppPoolTask
    {
        private string _appPoolName;

        private bool _failIfNotExist;

        public DeleteAppPoolTask(string appPoolName)
        {
            this._appPoolName = appPoolName;
        }

        public IDeleteAppPoolTask FailIfNotExist()
        {
            this._failIfNotExist = true;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                ApplicationPoolCollection applicationPoolCollection = serverManager.ApplicationPools;

                foreach (ApplicationPool applicationPool in applicationPoolCollection)
                {
                    if (applicationPool.Name == this._appPoolName)
                    {
                        applicationPoolCollection.Remove(applicationPool);
                        serverManager.CommitChanges();

                        context.LogInfo($"Application pool '{this._appPoolName}' has been deleted.");

                        return 0;
                    }
                }

                if (this._failIfNotExist)
                {
                    throw new TaskExecutionException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Application '{0}' does not exist.",
                            this._appPoolName), 1);
                }

                context.LogInfo($"Application pool '{this._appPoolName}' does not exist, doing nothing.");
                return 0;
            }
        }
    }
}
