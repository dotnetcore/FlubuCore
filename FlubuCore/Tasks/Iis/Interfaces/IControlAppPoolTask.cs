namespace FlubuCore.Tasks.Iis
{
    public interface IControlAppPoolTask
    {
        /// <summary>
        /// Name of the application pool to be controller. 
        ///  </summary>
        string ApplicationPoolName { get; set; }

        /// <summary>
        /// Action to be taken.
        /// </summary>
        ControlApplicationPoolAction Action { get; set; }

        /// <summary>
        /// If <c>true</c> task fails with exception if application pool doesn't exists. Otherwise not.
        /// </summary>
        bool FailIfNotExist { get; set; }
    }
}
