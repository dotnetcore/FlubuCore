using FlubuCore.Context.FluentInterface.Interfaces;namespace FlubuCore.Context
{
    public interface ITaskContext : IBuildPropertiesContext
    {
        /// <summary>
        /// Fluent interface for creating .net core tasks.
        /// </summary>
        /// <returns></returns>
        ICoreTaskFluentInterface CoreTasks();

        /// <summary>
        /// Fluent interface for creating tasks.
        /// </summary>
        /// <returns></returns>
        ITaskFluentInterface Tasks();

        /// <summary>
        /// Fluent interface for target creation. It creates The target and Add's it to the TargetTree.
        /// </summary>
        /// <param name="name">The target name.</param>
        /// <returns></returns>
        ITargetFluentInterface CreateTarget(string name);

        void LogInfo(string message);

        void LogError(string message);
    }
}
