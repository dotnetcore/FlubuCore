namespace FlubuCore.Tasks.Iis
{
    public interface IControlAppPoolTask
    {
        string ApplicationPoolName { get; set; }

        ControlApplicationPoolAction Action { get; set; }

        bool FailIfNotExist { get; set; }
    }
}
