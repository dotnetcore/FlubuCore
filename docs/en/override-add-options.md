FlubuCore offers you to add or override options in tasks that run external processes. 

`/o:{external_process_option}` 

With `/o:` FlubuCore adds specified option to all tasks in target's which can be a problem if target execute multiple tasks. So instead of `/o:`  you can use prefix by task name 

`/{taskName}:{external_process_option}`  

Alternatively you can change default prefix on task

```c#
    context.CreateTarget("Publish")
        AddCoreTask(x => x.Publish()
            .ChangeDefaultAdditionalOptionPrefix("/p:"));
```
## **Example**

Let's say you have target
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
