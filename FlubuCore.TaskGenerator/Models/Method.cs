namespace FlubuCore.TaskGenerator.Models
{
    public class Method
    {
        public string MethodName { get; set; }

        public string MethodSummary { get; set; }

        public string ArgumentKey { get; set; }

        public bool HasArgumentValue => !string.IsNullOrEmpty(ArgumentValue);

        public string ArgumentValue { get; set; }

    }
}
