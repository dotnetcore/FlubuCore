using Microsoft.Extensions.DependencyInjection;
using System;

namespace flubu.Infrastructure
{
    public static class Bootstrapper
    {
        private static IServiceCollection _services = new ServiceCollection();

        public static void Init()
        {
            //todo implement
            throw new NotImplementedException();
            //_services.AddSingleton<I>
        }

        public static void Run()
        {
            //todo implement
            throw new NotImplementedException();
        }
    }
}
