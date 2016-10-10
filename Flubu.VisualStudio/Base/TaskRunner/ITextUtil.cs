namespace Flubu.VisualStudio.Base.TaskRunner
{
    public interface ITextUtil
    {
        Range CurrentLineRange { get; }

        bool Delete(Range range);

        bool Insert(Range position, string text, bool addNewline);

        bool TryReadLine(out string line);

        string ReadAllText();

        void Reset();

        void FormatRange(LineRange range);
    }
}
