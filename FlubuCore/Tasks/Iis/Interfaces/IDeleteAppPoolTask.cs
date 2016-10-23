namespace FlubuCore.Tasks.Iis
{
    public interface IDeleteAppPoolTask : ITask
    {
        string ApplicationPoolName { get; set; }

        bool FailIfNotExist { get; set; }
    }
}
