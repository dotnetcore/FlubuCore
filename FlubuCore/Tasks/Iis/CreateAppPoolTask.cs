using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using Microsoft.Web.Administration;

namespace FlubuCore.Tasks.Iis
{
    public class CreateAppPoolTask : TaskBase<int>, ICreateAppPoolTask
    {
        private string _applicationPoolName;

        private CreateApplicationPoolMode _mode;

        public string ApplicationPoolName
        {
            get { return _applicationPoolName; }
            set { _applicationPoolName = value; }
        }

        public bool ClassicManagedPipelineMode { get; set; }

        public CreateApplicationPoolMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                ApplicationPoolCollection applicationPoolCollection = serverManager.ApplicationPools;

                ApplicationPool appPoolToWorkOn = null;
                bool updatedExisting = false;

                foreach (ApplicationPool applicationPool in applicationPoolCollection)
                {
                    if (applicationPool.Name == _applicationPoolName)
                    {
                        if (_mode == CreateApplicationPoolMode.DoNothingIfExists)
                        {
                            context.LogInfo($"Application pool '{_applicationPoolName}' already exists, doing nothing.");
                        }
                        else if (_mode == CreateApplicationPoolMode.FailIfAlreadyExists)
                        {
                            throw new TaskExecutionException(
                                string.Format(
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    "Application '{0}' already exists.",
                                    _applicationPoolName), 1);
                        }

                        // otherwise we should update the existing application pool
                        appPoolToWorkOn = applicationPool;
                        updatedExisting = true;
                        break;
                    }
                }

                if (appPoolToWorkOn == null)
                    appPoolToWorkOn = serverManager.ApplicationPools.Add(_applicationPoolName);

                appPoolToWorkOn.AutoStart = true;
                appPoolToWorkOn.Enable32BitAppOnWin64 = true;
                appPoolToWorkOn.ManagedPipelineMode =
                    ClassicManagedPipelineMode ? ManagedPipelineMode.Classic : ManagedPipelineMode.Integrated;
                ////serverManager.ApplicationPools.Add(appPoolToWorkOn);
                serverManager.CommitChanges();

                context.LogInfo(string.Format("Application pool '{0}' {1}.", _applicationPoolName, updatedExisting ? "updated" : "created"));
            }

            return 0;
        }
    }
}
