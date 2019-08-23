FlubuCore offers you to add or override options in **all** tasks that run external processes. 

Let's say you have target
```c#
context.CreateTarget("Example")`
    .AddCoreTask(x => x.Build("MySolution.sln").Configuration("Release"); 

```

and you wan't to build solution in debug configuration.
You could just write in console

`flubu example --configuration=Debug`

flubu would execute 

`dotnet build MySolution.sln --configuration Debug`

!!! info "Note"
	option keys are the same as in external processes. short versions of options keys also work. So in above example `-c=debug` would also work.
	
	FlubuCore also support tab completion for all options in tasks that run's external processes in [Interactive mode](build-script-runner-interactive.md)

<br/>

Tasks in FlubuCore plugins that does not support overriding of options out of the box can still be overriden with special prefix before option key `/o:`

`/o:{external_process_option={value}` 

With `/o:` FlubuCore adds specified option to all tasks in target's which can be a problem if target execute multiple tasks. So instead of `/o:`  you can use prefix by task name 

`/{taskName}:{external_process_option}={value}`  

Alternatively you can change default prefix on task

```c#
    context.CreateTarget("Publish")
        AddCoreTask(x => x.Publish()
            .ChangeDefaultAdditionalOptionPrefix("/p:"));
```

#### Example

Build task does support overriding of options out of the box but for the simplicity of the example build task is used.

```c#
context.CreateTarget("Example")`
    .AddCoreTask(x => x.Build("MySolution.sln").Configuration("Release"); 
```

and you wan't to build solution in debug configuration.
You could just write in console

`flubu example /o:configuration=Debug`

or
`flubu example /o:c=Debug`

or
`flubu example /build:c=Debug`

flubu would execute 

`dotnet build MySolution.sln -c Debug`
