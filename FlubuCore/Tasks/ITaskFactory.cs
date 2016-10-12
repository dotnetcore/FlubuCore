namespace FlubuCore.Tasks
{
    public interface ITaskFactory
    {
        T Create<T>()
            where T : TaskBase;

        T Create<T>(params object[] constructorArgs)
            where T : TaskBase;
    }
}
