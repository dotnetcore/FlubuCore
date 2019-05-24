using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.TaskGenerator.Models;

namespace FlubuCore.TaskGenerator
{
    public class TaskGeneratorBase
    {
        protected internal virtual string WriteParameters(List<Parameter> parameters)
        {
            string ret = string.Empty;
            foreach (var parameter in parameters)
            {
               
                if (!string.IsNullOrEmpty(ret))
                {
                    ret = $"{ret}, ";
                }

                ret = $"{ret} {WriteParameter(parameter)} ";
                
            }

            ret = ret.Trim();
            return ret;
        }

        protected internal virtual string WriteParameter(Parameter parameter)
        {
            if (parameter == null)
            {
                return string.Empty;
            }
            
            string parameterName = ParameterName(parameter.ParameterName);
            string parameterType = parameter.AsParams ? $"{parameter.ParameterType}[]" : parameter.ParameterType;
            string prms = parameter.AsParams ? "params " : string.Empty;
            string optional = parameter.IsOptional ? $" = {parameter.OptionalValue}" : string.Empty;
            return $"{prms}{parameterType} {parameterName}";
        }

        protected internal virtual string ParameterName(string parameterName)
        {
            if (parameterName.Equals("namespace") || parameterName.Equals("params") || parameterName.Equals("operator") ||
                parameterName.Equals("new") || parameterName.Equals("override"))
            {
                parameterName = $"@{parameterName}";
            }

            return parameterName;
        }
    }
}
