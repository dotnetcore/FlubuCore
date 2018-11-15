namespace FlubuCore.TaskGenerator.Models
{
    public class Parameter
    {
        public string ParameterType { get; set; }

        public string ParameterName { get; set; }

        /// <summary>
        /// Determinates if parameter is optional or not.
        /// </summary>
        public bool IsOptional { get; set; }

        public string OptionalValue { get; set; } = "null";

        /// <summary>
        /// Adds argument as params parameter to constructor.
        /// </summary>
        public bool AsParams {get; set; }
    }
}
