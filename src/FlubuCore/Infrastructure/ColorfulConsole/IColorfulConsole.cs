using System;

namespace FlubuCore.Infrastructure.ColorfulConsole
{
    public interface IColorfulConsole
    {
        void Flush();

        void Write(string message, ConsoleColor? background, ConsoleColor? foreground);

        void WriteLine(string message, ConsoleColor? background, ConsoleColor? foreground);
    }
}
