using FlubuCore.Context;

namespace FlubuCore.Targeting
{
    public static class TargetTreeExtensions
    {
        public static ITarget CreateTarget(ITaskSession session, string name)
        {
            return session.TargetTree.AddTarget(name);
        }
    }
}
