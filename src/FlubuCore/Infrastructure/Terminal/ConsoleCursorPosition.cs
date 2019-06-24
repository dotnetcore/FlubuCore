namespace FlubuCore.Infrastructure.Terminal
{
    internal class ConsoleCursorPosition
    {
        private readonly int _startLeft;
        private readonly int _startTop;
        private readonly int _windowWidth;

        public ConsoleCursorPosition(int startLeft, int startTop, int windowWidth)
        {
            _startLeft = startLeft;
            _startTop = startTop;
            _windowWidth = windowWidth;
        }

        public int Left { get; private set; }

        public int Top { get; private set; }

        public int InputLength { get; private set; }

        public int StartTop => _startTop;

        public static ConsoleCursorPosition operator +(ConsoleCursorPosition left, int value)
        {
            var newLength = left._startLeft + left.InputLength + value;
            var rows = newLength / left._windowWidth;
            var column = newLength - (rows * left._windowWidth);
            return new ConsoleCursorPosition(left._startLeft, left._startTop, left._windowWidth)
            {
                Left = column,
                Top = left._startTop + rows,
                InputLength = left.InputLength + value
            };
        }

        public static ConsoleCursorPosition operator ++(ConsoleCursorPosition left)
        {
            return left + 1;
        }

        public static ConsoleCursorPosition operator -(ConsoleCursorPosition left, int value)
        {
            return left + -value;
        }

        public static ConsoleCursorPosition operator --(ConsoleCursorPosition left)
        {
            return left - 1;
        }

        public ConsoleCursorPosition SetLength(int length)
        {
            var newLength = _startLeft + length;
            var rows = newLength / _windowWidth;
            var column = newLength - (rows * _windowWidth);
            return new ConsoleCursorPosition(_startLeft, _startTop, _windowWidth)
            {
                Left = column,
                Top = _startTop + rows,
                InputLength = length
            };
        }
    }
}