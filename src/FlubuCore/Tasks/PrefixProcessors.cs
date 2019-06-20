using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks
{
    public static class PrefixProcessors
    {
        public static string AddSlashPrefixToAdditionalOptionKey(string key)
        {
            if (key == null || !char.IsLetterOrDigit(key[0]))
            {
                return key;
            }

            return $"/{key}";
        }

        public static string AddDoubleDashPrefixToAdditionalOptionKey(string key)
        {
            if (key == null || !char.IsLetterOrDigit(key[0]))
            {
                return key;
            }

            return key.Length == 1 ? $"-{key}" : $"--{key}";
        }

        public static string AddSingleDashPrefixToAdditionalOptionKey(string key)
        {
            if (key == null || !char.IsLetterOrDigit(key[0]))
            {
                return key;
            }

            return $"--{key}";
        }
    }
}
