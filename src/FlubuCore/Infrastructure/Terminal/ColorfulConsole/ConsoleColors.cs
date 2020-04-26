using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Infrastructure.Terminal.ColorfulConsole
{
    public struct ConsoleColors
    {
        public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
        {
            Foreground = foreground;
            Background = background;
        }

        public ConsoleColor? Foreground { get; }

        public ConsoleColor? Background { get; }
    }
}
