namespace FlubuCore.Tasks.Iis
{
    public interface IDeleteAppPoolTask : ITask
    {
        /// <summary>
        /// Name of the application pool to be deleted
        /// </summary>
        string ApplicationPoolName { get; set; }

        /// <summary>
        /// If <c>true</c> task fails with exception if application pool doesn't exists. Otherwise not.
        /// </summary>
        bool FailIfNotExist { get; set; }
    }
}
