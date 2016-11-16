using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using Microsoft.Web.Administration;

namespace FlubuCore.Tasks.Iis
{
    public class DeleteAppPoolTask : TaskBase<int>, IDeleteAppPoolTask
    {
        public string ApplicationPoolName { get; set; }

        public bool FailIfNotExist { get; set; }

        protected override int DoExecute(ITaskContext context)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                ApplicationPoolCollection applicationPoolCollection = serverManager.ApplicationPools;

                foreach (ApplicationPool applicationPool in applicationPoolCollection)
                {
                    if (applicationPool.Name == ApplicationPoolName)
                    {
                        applicationPoolCollection.Remove(applicationPool);
                        serverManager.CommitChanges();

                        context.LogInfo($"Application pool '{ApplicationPoolName}' has been deleted.");

                        return 0;
                    }
                }

                if (FailIfNotExist)
                {
                    throw new TaskExecutionException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Application '{0}' does not exist.",
                            ApplicationPoolName), 1);
                }

                context.LogInfo($"Application pool '{ApplicationPoolName}' does not exist, doing nothing.");
                return 0;
            }
        }
    }
}
