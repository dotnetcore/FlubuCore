using System.IO;
using System.Reflection;

namespace FlubuCore.Tests
{
    public static class TestExtensions
    {
        public static string ExpandToExecutingPath(this string relativePath)
        {
            string location = typeof(InfrastructureTests).GetTypeInfo().Assembly.Location;
            string dirPath = Path.GetDirectoryName(location);
            return Path.Combine(dirPath, relativePath);
        }
    }
}
