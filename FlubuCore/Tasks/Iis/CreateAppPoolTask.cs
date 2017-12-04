using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using Microsoft.Web.Administration;

namespace FlubuCore.Tasks.Iis
{
    public class CreateAppPoolTask : TaskBase<int, CreateAppPoolTask>, ICreateAppPoolTask
    {
        private string _applicationPoolName;

        private bool _classicManagedPipelineMode;

        private CreateApplicationPoolMode _mode;

        private string _managedRuntimeVersion;
        private string _description;

        public CreateAppPoolTask(string applicationPoolName)
        {
            this._applicationPoolName = applicationPoolName;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Creates application pool {_applicationPoolName}";
                }

                return _description;
            }
            set { _description = value; }
        }

        public ICreateAppPoolTask UseClassicManagedPipelineMode()
        {
            this._classicManagedPipelineMode = true;
            return this;
        }

        public ICreateAppPoolTask Mode(CreateApplicationPoolMode  mode)
        {
            this._mode = mode;
            return this;
        }

        public ICreateAppPoolTask ManagedRuntimeVersion(string managedRuntimeVersion)
        {
            this._managedRuntimeVersion = managedRuntimeVersion;
            return this;
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
                appPoolToWorkOn.ManagedPipelineMode = _classicManagedPipelineMode ? ManagedPipelineMode.Classic : ManagedPipelineMode.Integrated;
                if (!string.IsNullOrEmpty(this._managedRuntimeVersion))
                {
                    appPoolToWorkOn.ManagedRuntimeVersion = this._managedRuntimeVersion;
                }
                ////serverManager.ApplicationPools.Add(appPoolToWorkOn);
                serverManager.CommitChanges();

                context.LogInfo(string.Format("Application pool '{0}' {1}.", _applicationPoolName, updatedExisting ? "updated" : "created"));
            }

            return 0;
        }
    }
}
