namespace FlubuCore.Tasks.Iis
{
    public interface IControlAppPoolTask : ITaskOfT<int, IControlAppPoolTask>
    {
        IControlAppPoolTask ApplicationPoolName(string appPoolName);

        /// <summary>
        /// If <c>true</c> task fails with exception if application pool doesn't exists. Otherwise not.
        /// </summary>
        IControlAppPoolTask FailIfNotExist();

        IControlAppPoolTask ForServer(string serverName);
    }
}
