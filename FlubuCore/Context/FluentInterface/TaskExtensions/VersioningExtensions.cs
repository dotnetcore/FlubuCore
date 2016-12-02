using FlubuCore.Context.FluentInterface.Interfaces;

namespace FlubuCore.Context.FluentInterface.TaskExtensions
{
    public partial class TaskExtensionsFluentInterface
    {
        public ITaskExtensionsFluentInterface UpdateDotnetVersion(string[] projectFiles, string[] additionalProps)
        {
            Target.AddCoreTask(x => x.UpdateNetCoreVersionTask(projectFiles)
                .AdditionalProp(additionalProps));

            return this;
        }
    }
}
