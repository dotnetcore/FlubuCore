### **Debug script through .net core console application**

The easiest way to debug your script is that you place your script in a console application and execute it in console application. After that you can debug your script as any other application.

How to execute scripts through console application can be found [here.](/execute-script-in-console-app-or-pack-as-global-tool/)

### **Writing build script tests and debuging build script through test**

Wiki coming soon. Meanwhile see simple [test on github](https://github.com/flubu-core/examples/blob/master/NetCore_csproj/BuildScript/BuildScriptTests.cs) to get you started: 

If needed you can debug build script through test.

You can use flubu task in other .net applications just like in above test example.

### **Debugging build script by attaching to running process**
You can debug build script by attaching debuger to Flubu process. 

* Because Flubu alters build script slightly you have to disable option 'Require source code files to exactly match the original version' in visual studio.
Option can be found under Tools->Options->Debugging->General->Require source code files to exactly match the original version. Not sure for VS code if any settings have to be changed.
* It is advised to use  WaitForDebugger extension method on ITaskContext before first break point

        protected override void ConfigureTargets(ITaskContext context)
        {
            context.WaitForDebugger();
        }

* Run build script and attach debugger to FlubuCore process. FlubuCore process name vary depending on which FlubuCore "runner" you are using.

	* FlubuCore.Runner - You have to attach debugger to process named flubu.exe
    * dotnet-flubu Cli tool - You have to attach debugger to right process named dotnet
	* FlubuCore.GlobalTool - You have to attach debugger to process named Flubu