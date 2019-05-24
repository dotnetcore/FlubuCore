namespace Flubu.VisualStudio.Base.TaskRunner
{
    public static class TextUtilExtensions
    {
        public static bool Replace(this ITextUtil util, Range range, string text)
        {
            //Deletes the current bindings elements and move it to the top of the file
            return util.Delete(range) && util.Insert(range, text, false);
        }
    }
}
