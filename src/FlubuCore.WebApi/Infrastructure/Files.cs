using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.WebApi.Infrastructure
{
    public static class Files
    {
        public static string GetFileNameFromConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return null;
            }

            var startIndex = connectionString.IndexOf("FileName=", StringComparison.OrdinalIgnoreCase);

            if (startIndex == -1)
            {
                return null;
            }

            var fileName = connectionString.Substring(startIndex, connectionString.Length - startIndex);
            fileName = fileName.Substring(fileName.IndexOf('=') + 1);
            if (fileName.Contains(" "))
            {
                fileName = fileName.Substring(0, fileName.IndexOf(' '));
            }

            if (fileName.Contains(";"))
            {
                fileName = fileName.Substring(0, fileName.IndexOf(';'));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            return fileName;
        }
    }
}
