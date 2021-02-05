using System.Drawing;
using FlubuCore.Context.FluentInterface.Interfaces;

namespace FlubuCore.Context
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
        ITarget CreateTarget(string name);

        /// <summary>
        /// Interaction with various build servers(continous integration / continous delivery servers).
        /// </summary>
        /// <returns></returns>
        IBuildServer BuildServers();

        void LogInfo(string message);

        void LogInfo(string message, Color foregroundColor);

        void LogError(string message);

        void LogError(string message, Color foregroundColor);
    }
}
