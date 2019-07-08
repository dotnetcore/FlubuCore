FlubuCore 允许在运行外部程序（external processes）的任务中添加或覆盖选项。

`/o:{external_process_option}`

`/o:` 会为 FlubuCore 目标中所有任务添加置顶选项。如果目标执行多个任务，那么这可能是个问题，所以

`/{taskName}:{external_process_option}`

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
