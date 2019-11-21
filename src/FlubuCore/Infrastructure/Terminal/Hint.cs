using System;

namespace FlubuCore.Infrastructure.Terminal
{
    public class Hint
    {
        public string Name { get; set; }

        public string Help { get; set; }

        public HintType HintType { get; set; } = HintType.Default;

        public ConsoleColor HintColor { get; set; } = ConsoleColor.DarkGray;

        public bool OnlySimpleSearh { get; set; }
    }
}
