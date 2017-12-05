namespace FlubuCore.Tasks.Utils
{
    public enum StandardServiceControlCommands
    {
        /// <summary>
        /// Starts a windows service.
        /// </summary>
        Start,

        /// <summary>
        /// Sends a STOP request to a windows service.
        /// </summary>
        Stop,

        /// <summary>
        /// Creates a windows service.
        /// </summary>
        Create,

        /// <summary>
        /// Deletes a windows service.
        /// </summary>
        Delete,
    }
}
