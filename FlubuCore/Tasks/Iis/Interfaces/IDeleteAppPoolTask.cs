namespace FlubuCore.Tasks.Iis
{
    public interface IDeleteAppPoolTask : ITaskOfT<int>
    {
        /// <summary>
        /// task fails with exception if application pool doesn't exists. Otherwise not.
        /// </summary>
        IDeleteAppPoolTask FailIfNotExist();
    }
}
