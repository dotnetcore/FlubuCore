using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using FlubuCore.Context;
using Microsoft.Web.Administration;

namespace FlubuCore.Tasks.Iis
{
    public class ControlAppPoolTask : TaskBase<int, IControlAppPoolTask>, IControlAppPoolTask
    {
        private readonly ControlApplicationPoolAction _action;

        private string _applicationPoolName;

        private bool _failIfNotExist;
        private string _description;
        private string _serverName;

        public ControlAppPoolTask(string applicationPoolName, ControlApplicationPoolAction action)
        {
            _applicationPoolName = applicationPoolName;
            _action = action;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"{_action.ToString()}s application pool {_applicationPoolName}.";
                }

                return _description;
            }

            set { _description = value; }
        }

        public IControlAppPoolTask ApplicationPoolName(string appPoolName)
        {
            _applicationPoolName = appPoolName;
            return this;
        }

        public IControlAppPoolTask FailIfNotExist()
        {
            _failIfNotExist = true;
            return this;
        }

        public IControlAppPoolTask ForServer(string serverName)
        {
            _serverName = serverName;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            ServerManager serverManager = string.IsNullOrEmpty(_serverName)
                ? new ServerManager()
                : ServerManager.OpenRemote(_serverName);

            using (serverManager)
            {
                ApplicationPoolCollection applicationPoolCollection = serverManager.ApplicationPools;
                foreach (ApplicationPool applicationPool in applicationPoolCollection)
                {
                    if (applicationPool.Name == _applicationPoolName)
                    {
                        switch (_action)
                        {
                            case ControlApplicationPoolAction.Start:
                                {
                                    RunWithRetries(x => applicationPool.Start(), 3);
                                    break;
                                }

                            case ControlApplicationPoolAction.Stop:
                                {
                                    RunWithRetries(
                                        x => applicationPool.Stop(),
                                        3,
                                        -2147023834 /*app pool already stopped*/);
                                    break;
                                }

                            case ControlApplicationPoolAction.Recycle:
                                {
                                    RunWithRetries(x => applicationPool.Recycle(), 3);
                                    break;
                                }

                            default:
                                throw new NotSupportedException();
                        }

                        serverManager.CommitChanges();

                        DoLogInfo($"Application pool '{_applicationPoolName}' has been {_action}ed.");
                        return 0;
                    }
                }

                string appPoolDoesNotExistMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Application pool '{0}' does not exist.",
                    _applicationPoolName);

                if (_failIfNotExist)
                    throw new TaskExecutionException(appPoolDoesNotExistMessage, 1);

                DoLogInfo($"No action taken on application pool {_applicationPoolName}");
                return 0;
            }
        }

        private static void RunWithRetries(
            Action<int> action,
            int retries,
            params long[] ignoredErrorCodes)
        {
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    action(0);
                    break;
                }
                catch (COMException ex)
                {
                    #if !NETSTANDARD1_6
                    for (int j = 0; j < ignoredErrorCodes.Length; j++)
                    {
                        if (ignoredErrorCodes[j] == ex.ErrorCode)
                        {
                            return;
                        }
                    }
                    #endif
                    if (i == retries - 1)
                        throw;
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
