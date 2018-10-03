#r ".\FlubuCore.TaskGenerator.dll"

var sc = new FlubuCore.TaskGenerator.TaskGenerator(Context);
sc.GenerateTask(new FlubuCore.TaskGenerator.Models.Task
{
    FileName = "b.cs",
    ProjectName = "Tests",
    TaskName = "TestTask",
    Namespace = "FlubuCore.TaskGenerator.Tests",
    Constructor = new FlubuCore.TaskGenerator.Models.Constructor
    {
        Arguments = new System.Collections.Generic.List<FlubuCore.TaskGenerator.Models.Argument>
        {
            new FlubuCore.TaskGenerator.Models.Argument
            {
                ArgumentKey = "Test",
                Parameter = new FlubuCore.TaskGenerator.Models.Parameter
                {
                    ParameterType ="string",
                    ParameterName = "param1",
                }
            }
        },
    },
    Methods = new System.Collections.Generic.List<FlubuCore.TaskGenerator.Models.Method>
    {
        new FlubuCore.TaskGenerator.Models.Method
        {
            MethodName = "Method1",
            MethodSummary = "Description description!",
            Argument = new FlubuCore.TaskGenerator.Models.Argument
            {
                ArgumentKey = "--arg"                
            }
        },

        new FlubuCore.TaskGenerator.Models.Method
        {
            MethodName = "Method2",
            Argument = new FlubuCore.TaskGenerator.Models.Argument
            {
                ArgumentKey = "--arg",
                HasArgumentValue = true,
                Parameter = new FlubuCore.TaskGenerator.Models.Parameter
                {
                    ParameterType = "string",
                    ParameterName = "param1"
                }
            }
        }

    }
})