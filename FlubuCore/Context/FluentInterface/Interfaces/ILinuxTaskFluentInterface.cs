using FlubuCore.Tasks.Linux;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ILinuxTaskFluentInterface
    {
        /// <summary>
        /// Run's system ctl.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        SystemCtlTask SystemCtlTask(string command, string service);

        /// <summary>
        /// Run specified command on the remote host.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        SshCommandTask SshCommand(string host, string username, string password);

        /// <summary>
        /// Run specified command on the remote host.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        SshCommandTask SshCommand(string host, string username);

        /// <summary>
        /// Copy projects/files to the remote host.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        SshCopyTask SshCopy(string host, string username, string password);

        /// <summary>
        /// Copy projects/files to the remote host.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        SshCopyTask SshCopy(string host, string username);
    }
}
