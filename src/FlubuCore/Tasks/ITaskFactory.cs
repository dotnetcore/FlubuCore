namespace FlubuCore.Tasks
{
    public interface ITaskFactory
    {
        T Create<T>()
            where T : ITask;

        T Create<T>(params object[] constructorArgs)
            where T : ITask;
    }
}
