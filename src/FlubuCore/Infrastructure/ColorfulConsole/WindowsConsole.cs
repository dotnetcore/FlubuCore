// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Basded on WindowsLogConsole.cs at https://github.com/aspnet/Logging/tree/8270c545/src/Microsoft.Extensions.Logging.Console/Internal

using System;
using System.IO;

namespace FlubuCore.Infrastructure.ColorfulConsole
{
    public class WindowsConsole : IColorfulConsole
    {
        private readonly TextWriter _textWriter;

        public WindowsConsole(bool stdErr = false)
        {
            _textWriter = stdErr ? Console.Error : Console.Out;
        }

        private bool SetColor(ConsoleColor? background, ConsoleColor? foreground)
        {
            if (background.HasValue)
            {
                Console.BackgroundColor = background.Value;
            }

            if (foreground.HasValue)
            {
                Console.ForegroundColor = foreground.Value;
            }

            return background.HasValue || foreground.HasValue;
        }

        private void ResetColor()
        {
            Console.ResetColor();
        }

        public void Write(string message, ConsoleColor? background, ConsoleColor? foreground)
        {
            var colorChanged = SetColor(background, foreground);
            _textWriter.Write(message);
            if (colorChanged)
            {
                ResetColor();
            }
        }

        public void WriteLine(string message, ConsoleColor? background, ConsoleColor? foreground)
        {
            var colorChanged = SetColor(background, foreground);
            _textWriter.WriteLine(message);
            if (colorChanged)
            {
                ResetColor();
            }
        }

        public void Flush()
        {
            // No action required as for every write, data is sent directly to the console
            // output stream
        }
    }
}
