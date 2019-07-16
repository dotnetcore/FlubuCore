你可以通过编写自己的 flubu 任务来扩展 flubu 的流畅接口（fluent interface）。

当你通过自定义任务扩展 FlubuCore 时，只需要将其添加到目标（target）或通过 Do 来调用它即可，以下是例子：

```c#
public class BuildScript : DefaultBuildScript
{
    protected override void ConfigureTargets(ITaskContext context)
    {
        context.CreateTarget("FlubuPlugin.Example")
           .SetAsDefault()
           .Do(DoPluginExample);

       context.CreateTarget("FlubuPlugin.Example2")
           .AddTask(x => x.ExampleFlubuPluginTask());
    }

   private void DoPluginExample(ITaskContext context)
   {
       context.Tasks().ExampleFlubuPluginTask()
           .Message("some example message from plugin").Execute(context);
   }
}
```

### **如何创建任务插件**

- 在 VS 中创建新项目，项目名 FlubuCore.{PluginName}
- 在项目中引入 FlubuCore nuget 包
- 添加任务并实现之。以下代码演示了示例插件任务的实现：

```c#
public class ExampleFlubuPluginTask : TaskBase<int, ExampleFlubuPluginTask>
{
    private string _message;

    protected override string Description { get; set; }

    public ExampleFlubuPluginTask Message(string message)
    {
        _message = message;
        return this;
    }

    protected override int DoExecute(ITaskContextInternal context)
    {
        //// write task logic here.
        context.LogInfo(!string.IsNullOrEmpty(_message) ? _message : "Just some dummy code");

        return 0;
    }
}
```

- 然后，你需要编写一个扩展方法将任务添加对 flubu 流畅接口的支持。在本例中我们的扩展方法如下：

```C#
using FlubuCore.PluginExample;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
   public static class TaskFluentInterfaceExtension
    {
        public static ExampleFlubuPluginTask ExampleFlubuPluginTask(this ITaskFluentInterface flubu)
        {
            return new ExampleFlubuPluginTask();
        }
    }
}
```

- 建议你将任务添加到对 ICoreTaskFluentInterface 或 ITaskFluentInterface 的扩展支持。

- 如果你将插件添加到 nuget 仓库中为佳。如果插件名以 FlubuCore 开头则更佳，因为这样大家都能很容易找到这个插件。

- 你可以在[这里](https://github.com/flubu-core/examples/tree/master/FlubuCustomTaskPluginAndLoadAssembliesFromDirectoryExample)查看示例插件的完整代码。
