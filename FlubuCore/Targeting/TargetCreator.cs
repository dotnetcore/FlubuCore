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
    public static class TargetCreator
    {
        /// <summary>
        /// Searches methods with Target attribute in specified type and creates targets.
        /// </summary>
        /// <param name="buildScriptType"></param>
        /// <param name="taskSession"></param>
        public static void CreateTargetFromMethodAttributes(IBuildScript buildScript, ITaskSession taskSession)
        {
            #if !NETSTANDARD1_6
            var buildScriptType = buildScript.GetType();
            var methods = buildScriptType.GetMethods().Where(x => x.DeclaringType == buildScriptType).ToList();
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
                        throw new ScriptException($"Failed to create target '{attribute.TargetName}'. Method '{methodInfo.Name}' must have atleast one parameter which must be of type '{nameof(ITargetFluentInterface)}'");
                    }

                    if (methodParameters[0].ParameterType != typeof(ITargetFluentInterface))
                    {
                        throw new ScriptException($"Failed to create target '{attribute.TargetName}' first parameter in method '{methodInfo.Name}' must be of type '{nameof(ITargetFluentInterface)}'");
                    }

                    var target = taskSession.CreateTarget(attribute.TargetName);
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

                    methodInfo.Invoke(buildScript, attributeParamaters.ToArray());
                }
            }
#endif
        }
    }
}
