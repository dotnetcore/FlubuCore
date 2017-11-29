using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using FlubuCore.Context;
using Microsoft.Web.Administration;

namespace FlubuCore.Tasks.Iis
{
    public class ControlAppPoolTask : TaskBase<int, IControlAppPoolTask>, IControlAppPoolTask
    {
        private string _applicationPoolName;

        private ControlApplicationPoolAction _action;

        private bool _failIfNotExist;
        private string _description;

        public ControlAppPoolTask(string applicationPoolName, ControlApplicationPoolAction action)
        {
            this._applicationPoolName = applicationPoolName;
            this._action = action;
        }

        public IControlAppPoolTask FailIfNotExist()
        {
            this._failIfNotExist = true;
            return this;
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


        protected override int DoExecute(ITaskContextInternal context)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                ApplicationPoolCollection applicationPoolCollection = serverManager.ApplicationPools;
                const string Message = "Application pool '{0}' has been {1}ed.";
                foreach (ApplicationPool applicationPool in applicationPoolCollection)
                {
                    if (applicationPool.Name == this._applicationPoolName)
                    {
                        string logMessage;
                        switch (_action)
                        {
                            case ControlApplicationPoolAction.Start:
                                {
                                    RunWithRetries(x => applicationPool.Start(), 3);
                                    logMessage = string.Format(CultureInfo.InvariantCulture, Message, _applicationPoolName, _action);
                                    break;
                                }

                            case ControlApplicationPoolAction.Stop:
                                {
                                    RunWithRetries(
                                        x => applicationPool.Stop(),
                                        3,
                                        -2147023834 /*app pool already stopped*/);
                                    logMessage = string.Format(CultureInfo.InvariantCulture, Message, _applicationPoolName, "stopp");
                                    break;
                                }

                            case ControlApplicationPoolAction.Recycle:
                                {
                                    RunWithRetries(x => applicationPool.Recycle(), 3);
                                    logMessage = string.Format(CultureInfo.InvariantCulture, Message, _applicationPoolName, _action);
                                    break;
                                }

                            default:
                                throw new NotSupportedException();
                        }

                        serverManager.CommitChanges();

                        context.LogInfo(logMessage);
                        return 0;
                    }
                }

                string appPoolDoesNotExistMessage = string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "Application pool '{0}' does not exist.",
                    _applicationPoolName);

                if (_failIfNotExist)
                    throw new TaskExecutionException(appPoolDoesNotExistMessage, 1);

                context.LogInfo(Message);
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
                catch (COMException)
                {
                    //// todo ErrorCode is not available in .net core
                    ////for (int j = 0; j < ignoredErrorCodes.Length; j++)
                    ////{
                    ////    if (ignoredErrorCodes[j] == ex.ErrorCode
                    ////       return;
                    ////}

                    if (i == retries - 1)
                        throw;
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
