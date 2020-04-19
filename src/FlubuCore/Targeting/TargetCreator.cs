using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;

namespace FlubuCore.Targeting
{
    public class TargetCreator : ITargetCreator
    {
        /// <summary>
        /// Searches methods with Target attribute in specified type and creates targets.
        /// </summary>
        /// <param name="buildScriptType"></param>
        /// <param name="flubuSession"></param>
        public void CreateTargetFromMethodAttributes(IBuildScript buildScript, IFlubuSession flubuSession)
        {
            var buildScriptType = buildScript.GetType();
            var methods = buildScriptType.GetRuntimeMethods().Where(x => x.DeclaringType == buildScriptType).ToList();
            foreach (var methodInfo in methods)
            {
                var attributes = methodInfo.GetCustomAttributes<TargetAttribute>(false).ToList();

                if (attributes.Count == 0)
                {
                    continue;
                }

                foreach (var attribute in attributes)
                {
                    var methodParameters = methodInfo.GetParameters().ToList();

                    if (methodParameters.Count == 0)
                    {
                        throw new ScriptException($"Failed to create target '{attribute.TargetName}'. Method '{methodInfo.Name}' must have atleast one parameter which must be of type '{nameof(ITarget)}'");
                    }

                    if (methodParameters[0].ParameterType != typeof(ITarget))
                    {
                        throw new ScriptException($"Failed to create target '{attribute.TargetName}' first parameter in method '{methodInfo.Name}' must be of type '{nameof(ITarget)}'");
                    }

                    var target = flubuSession.CreateTarget(attribute.TargetName);
                    var attributeParamaters = new List<object>() { target };

                    attributeParamaters.AddRange(attribute.MethodParameters);

                    if (methodParameters.Count != attributeParamaters.Count)
                    {
                        throw new ScriptException($"Failed to create target '{attribute.TargetName}'. Method parameters {methodInfo.Name} do not match count of attribute parametrs.");
                    }

                    for (int i = 0; i < methodParameters.Count; i++)
                    {
                        if (i != 0 && methodParameters[i].ParameterType != attributeParamaters[i].GetType())
                        {
                            throw new ScriptException($"Failed to create target '{attribute.TargetName}'. Attribute parameter {i + 1.ToString()} does not match '{methodInfo.Name}' method parameter {i + 1.ToString()}. Expected {methodParameters[i].ParameterType} Actual: {attributeParamaters[i].GetType()}");
                        }
                    }

                    var parameterInfos = methodInfo.GetParameters();
                    for (int i = 0; i < parameterInfos.Length; i++)
                    {
                        ParameterInfo parameter = parameterInfos[i];
                        var paramAttributes = parameter.GetCustomAttributes<FromArgAttribute>(false).ToList();
                        foreach (var fromArgAttribute in paramAttributes)
                        {
                            if (!flubuSession.Args.ScriptArguments.ContainsKey(fromArgAttribute.ArgKey))
                            {
                                continue;
                            }

                            attributeParamaters[i] = MethodParameterModifier.ParseValueByType(flubuSession.Args.ScriptArguments[fromArgAttribute.ArgKey], parameter.ParameterType);
                        }

                        if (flubuSession.Args.ScriptArguments.ContainsKey(parameter.Name))
                        {
                            object parsedValue = MethodParameterModifier.ParseValueByType(flubuSession.Args.ScriptArguments[parameter.Name], parameter.ParameterType);
                            attributeParamaters[i] = parsedValue;
                        }
                    }

                    methodInfo.Invoke(buildScript, attributeParamaters.ToArray());
                }
            }
        }
    }
}
