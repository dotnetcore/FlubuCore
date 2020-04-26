??? question "Why FlubuCore?"
	- Native access  to the whole .NET ecosystem and full IDE support inside of your scripts.
    - With flubu you can execute your script anywhere it doesn't need to be in a project. 
	  This is important for deployment scripts or if you want to write some other scripts that are not releated to build scirpts or deploy scripts
    - Flubu allows multiple tasks in target.
	- Easy access to tasks through fluent interface `Context.AddTask()` or `Context.Tasks()`
	- Each Flubu built in task derives from base class task meaning each built in task have retry, OnError, Finally, When, Interactive and some others mechanisms
	- Pass command line arguments, settings from json configuration file or environment variables to your Properties in script.
	- Allows you to reuse set of tasks. See [Sample](https://github.com/dotnetcore/FlubuCore.Examples/blob/master/DeployScriptExample/BuildScript/DeployScript.cs)
	- flubu supports parallel / async execution of target's, target dependencies and tasks
	- to each task that execute external program or command you can add custom arguments with .WithArguments() method or even decide not to use fluent interface (good example: https://github.com/azabluda/InfoCarrier.Core/blob/develop/BuildScript/BuildScript.cs)
    - Alternative target definitions with attributes
	- Flubu web api allows you to execute scripts remotely (usefull for deployments but not limited to)
	- Override existing options or add additional options to tasks through console. https://flubucore.dotnetcore.xyz/override-add-options/
	- Flubu have really nice interactive mode https://flubucore.dotnetcore.xyz/build-script-runner-interactive/
	- Flubu Supports .net 461+ and .net core 1.0
??? question "Should I call Execute method when executing flubu built in task?"
	if you are adding task to target through `AddTask` method Flubu calls `Execute` method when executing that target so in this scenario you should not call `Execute` on the task
	```c#
		  context.CreateTarget("Build")
				 .AddCoreTask(x => x.build());
	```

	In the sample above `BuildTask` is added to the target. When target is executed Flubu executes all tasks  that were added  to target by calling task `Execute` method. in this case it executes `BuildTask`  

	```c#
	 context.CreateTarget("LoginEcr")
					 .Do(c =>
					 {
							c.Tasks()
							 .RunProgram("aws")	
							 .WithArguments("ecr", "get-login", "--region", "eu-central-1", "--no-include-email"))
							 .Execute(context);
					 }
	```

	In this sample `Do` actually adds  [DoTask](https://github.com/dotnetcore/FlubuCore/blob/master/src/FlubuCore/Tasks/DoTask.cs) to the target. When target is executed Flubu executes `DoTask`. DoTask in above example invokes Anonymous method which was assigned to the Action delegate (first parameter in Do Method). 
	Flubu can not execute by itself tasks in the anonymous method you have to call `Execute()` method manually. 
		
??? question "Can I get output of the program, process or command that I am executing with Flubu?"
	Yes you can with `CaptureOutput` method in `RunProgramTask`
	
	```c#
	public class MyScript : DefaultBuildScript
    {
        protected override void ConfigureTargets(ITaskContext context)
        {
            context.CreateTarget("Example")
                .Do(RunProgramOrCommandExample);
        }

        public void RunProgramOrCommandExample(ITaskContext context)
        {
            var task = context.Tasks().RunProgramTask("EnterPathToProgramOrCommand")
                .WithArguments("Add arguments if needed")
                .CaptureOutput();

            task.Execute(context);

            var output = task.GetOutput();
        }
    }
	```

??? question "Can I access Properties or flubu BuildProperties in ConfigureTargets method?"
	In most cases you can as long as they are not set in a `Do` method or in a task.
	
	
	```c#
    public BuildVersion BuildVersion { get; set; } = null;

    public int SimpleSample {get; set; } = 0;
	
    protected override void ConfigureTargets(ITaskContext context)
    {
       var fetchBuildVersion context.CreateTarget("fetch.buildVersion").Do(FetchBuildVersion);

        context.CreateTarget("Build")
			.DependsOn(fetchBuildVersion)
            .AddCoreTask(x => x.Build()
                .Version(BuildVersion.BuildVersionWithQuality())); /// BuildVersion is null here.

		context.LogInfo($"sample value: '{SimpleSample}'"); /// logs 0 and not 5. Explained below why is it so.
    }

    private void FetchBuildVersion(ITaskContext context)
    {
        BuildVersion = context.Tasks().FetchBuildVersionFromFileTask()
            .ProjectVersionFileName("project_version.txt")
            .Execute(context);
		
		SimpleSample = 5;
    }
	```
	
	In sample above you could think that when property `SimpleSample` is accessed in 	
	`ConfigureTargets` it would not be 0 but it is becuase `ConfigureTargets` method
	is always executed before all targets that are executed with flubu
	
	!!! note "ConfigureTargets is also executed before all target dependecnies and tasks that were added to target"
	
	