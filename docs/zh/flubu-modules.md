It is possible to change FlubuCore internal logic by  replacing FlubuCore modules through DI. For example logging, target runner, task context etc.. could be changed. 

```C#
public class BuildScript : DefaultBuildScript
{
  public override void ConfigureServices(IServiceCollection services)
  {
     services.Replace<IFlubuSession, MyFlubuSession>();
  }

  public override void Configure(ILoggerFactory loggerFactory)
  {
      loggerFactory.AddProvider(new MyLoggerProvider());
  }
}
```