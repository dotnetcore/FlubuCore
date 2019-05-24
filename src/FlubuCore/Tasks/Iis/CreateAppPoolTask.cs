using System.Globalization;
using FlubuCore.Context;
using Microsoft.Web.Administration;

namespace FlubuCore.Tasks.Iis
{
    public class CreateAppPoolTask : TaskBase<int, CreateAppPoolTask>, ICreateAppPoolTask
    {
        private string _applicationPoolName;

        private bool _classicManagedPipelineMode;
        private string _description;

        private string _managedRuntimeVersion;

        private CreateApplicationPoolMode _mode;

        public CreateAppPoolTask(string applicationPoolName)
        {
            _applicationPoolName = applicationPoolName;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description)) return $"Creates application pool {_applicationPoolName}";

                return _description;
            }
            set => _description = value;
        }

        public CreateAppPoolTask ApplicationPoolName(string applicationPoolName)
        {
            _applicationPoolName = applicationPoolName;
            return this;
        }

        public ICreateAppPoolTask UseClassicManagedPipelineMode()
        {
            _classicManagedPipelineMode = true;
            return this;
        }

        public ICreateAppPoolTask Mode(CreateApplicationPoolMode mode)
        {
            _mode = mode;
            return this;
        }

        public ICreateAppPoolTask ManagedRuntimeVersion(string managedRuntimeVersion)
        {
            _managedRuntimeVersion = managedRuntimeVersion;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            using (var serverManager = new ServerManager())
            {
                var applicationPoolCollection = serverManager.ApplicationPools;

                ApplicationPool appPoolToWorkOn = null;
                var updatedExisting = false;

                foreach (var applicationPool in applicationPoolCollection)
                {
                    if (applicationPool.Name == _applicationPoolName)
                    {
                        if (_mode == CreateApplicationPoolMode.DoNothingIfExists)
                        {
                            DoLogInfo(
                                $"Application pool '{_applicationPoolName}' already exists, doing nothing.");
                        }
                        else if (_mode == CreateApplicationPoolMode.FailIfAlreadyExists)
                        {
                            throw new TaskExecutionException(
                                string.Format(
                                    CultureInfo.InvariantCulture,
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
                appPoolToWorkOn.ManagedPipelineMode = _classicManagedPipelineMode
                    ? ManagedPipelineMode.Classic
                    : ManagedPipelineMode.Integrated;
                if (!string.IsNullOrEmpty(_managedRuntimeVersion))
                    appPoolToWorkOn.ManagedRuntimeVersion = _managedRuntimeVersion;
                ////serverManager.ApplicationPools.Add(appPoolToWorkOn);
                serverManager.CommitChanges();

                DoLogInfo($"Application pool '{_applicationPoolName}' {(updatedExisting ? "updated" : "created")}.");
            }

            return 0;
        }
    }
}