You can write your own tasks for flubu and extend flubu fluent interface with them.

When fluent interface will be extended with your custom task you could simply add it to the target or execute it with Do task with the following example code:

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
### **How to create your own task plugin**
* Create new project in vs FlubuCore.{PluginName}
* Add FlubuCore nuget package to project.
* Add task and implement it. Following code shows implementation of example flubu plugin task.

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

* Then you need to write an extension method to add the task to flubu fluent interface. Extension method for our example task:

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

* It is recommended that you add task to ICoreTaskFluentInterface or  ITaskFluentInterface

* We would be very glad if you add your plugin to the nuget repository. It would be great if the plugin name would start 
  with FlubuCore so others can find it.

* you can see whole example plugin code [here](https://github.com/flubu-core/examples/tree/master/FlubuCustomTaskPluginAndLoadAssembliesFromDirectoryExample)
 