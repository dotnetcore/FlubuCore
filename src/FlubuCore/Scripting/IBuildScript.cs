using FlubuCore.Context;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Scripting
{
    public interface IBuildScript
    {
        int Run(IFlubuSession flubuSession);

        void ConfigureServices(IServiceCollection services);
    }
}