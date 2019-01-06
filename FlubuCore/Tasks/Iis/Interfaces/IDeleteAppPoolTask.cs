namespace FlubuCore.Tasks.Iis
{
    public interface IDeleteAppPoolTask : ITaskOfT<int, IDeleteAppPoolTask>
    {
        /// <summary>
        /// Set's name of the appication pool to be deleted.
        /// </summary>
        /// <param name="appPoolName"></param>
        /// <returns></returns>
        IDeleteAppPoolTask ApplicationPoolName(string appPoolName);

        /// <summary>
        /// task fails with exception if application pool doesn't exists. Otherwise not.
        /// </summary>
        IDeleteAppPoolTask FailIfNotExist();
    }
}
