namespace FlubuCore.Tasks.Iis
{
    public interface IDeleteAppPoolTask : ITaskOfT<int, IDeleteAppPoolTask>
    {
        /// <summary>
        /// task fails with exception if application pool doesn't exists. Otherwise not.
        /// </summary>
        IDeleteAppPoolTask FailIfNotExist();
    }
}
