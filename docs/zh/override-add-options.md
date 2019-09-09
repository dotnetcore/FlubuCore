FlubuCore 允许在运行外部程序（external processes）的**所有**任务中添加或覆盖选项。

假设你有一个 target
```c#
context.CreateTarget("Example")`
    .AddCoreTask(x => x.Build("MySolution.sln").Configuration("Release"); 

```

并且你不想在调试配置中构建解决方案。

你可以直接在控制台中输入：

`flubu example --configuration=Debug`

FlubuCore 将执行： 

`dotnet build MySolution.sln --configuration Debug`

!!! info "Note"
	如果选项的 key 与外部程序的一样，那么可以使用对应的简短版本的 key。因此在上例中，`-c = debug` 也是可以的。

    FlubuCore 还支持在[交互模式](build-script-runner-interactive.md)中通过 Tab 键来自动补全信息。

<br/>

Tasks in FlubuCore plugins that does not support overriding of options out of the box can still be overriden with special prefix before option key `/o:`

`/o:{external_process_option}={value}`

`/o:` 会为 FlubuCore 目标中所有任务添加置顶选项。如果目标执行多个任务，那么这可能是个问题，所以

`/{taskName}:{external_process_option}={value}`

你也可以修改任务的默认前缀（default prefix）：

```c#
    context.CreateTarget("Publish")
        AddCoreTask(x => x.Publish()
            .ChangeDefaultAdditionalOptionPrefix("/p:"));
```

## **示例**

假设创建了目标（target）：

```c#
context.CreateTarget("Example")`
    .AddCoreTask(x => x.Build("MySolution.sln").Configuration("Release");

```

你不想在调试配置（debug configuration）中构建解决方案,

你可以在控制台中写

`flubu build /o:configuration=Debug`

或

`flubu build /o:c=Debug`

或

`flubu build /build:c=Debug`

flubu 将执行

`dotnet build MySolution.sln -c Debug`
