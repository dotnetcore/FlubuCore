using System;

namespace FlubuCore.Infrastructure.Terminal
{
    internal static class ConsoleUtils
    {
        public const string Prompt = "> ";

        public static void WriteLine(string line, ConsoleColor color)
        {
            var currentColor = Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.WriteLine(line);
            System.Console.ForegroundColor = currentColor;
        }

        public static void Write(string line, ConsoleColor color)
        {
            var currentColor = Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.Write(line);
            System.Console.ForegroundColor = currentColor;
        }

        public static void WritePrompt(string text)
        {
            Write($"{text}{Prompt}", ConsoleColor.Green);
        }
    }
}