namespace FlubuCore.Tasks
{
    public interface ITaskFactory
    {
        T Create<T>()
            where T : TaskMarker;

        T Create<T>(params object[] constructorArgs)
            where T : TaskMarker;
    }
}
