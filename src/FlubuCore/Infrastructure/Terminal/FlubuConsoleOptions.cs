using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Infrastructure.Terminal
{
    public class FlubuConsoleOptions
    {
        public bool OnlyDirectoriesSuggestions { get; set; } = false;

        public string InitialText { get; set; }

        public string InitialHelp { get; set; }

        public bool WritePrompt { get; set; } = true;

        public bool IncludeFileSuggestions { get; set; } = false;

        public string FileSuggestionsSearchPattern { get; set; } = "*.*";

        public string DefaultSuggestion { get; set; }
    }
}
