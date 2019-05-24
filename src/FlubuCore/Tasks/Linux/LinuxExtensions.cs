using System;
using System.Text;

namespace FlubuCore.Tasks.Linux
{
    internal static class LinuxExtensions
    {
        internal static string GetPassword(this string password)
        {
            if (!string.IsNullOrEmpty(password))
                return password;

            Console.Write("Please enter password:");

            var pwd = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }

                if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.Remove(pwd.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pwd.Append(i.KeyChar);
                    Console.Write("*");
                }
            }

            Console.WriteLine();
            return pwd.ToString();
        }
    }
}
