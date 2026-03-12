using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FlubuCore.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(o =>
                    {
                        o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
                    })
                    .UseIIS()
                    .UseStartup<Startup>();
                }).Build();

             webHostBuilder.Run();
        }
    }
}
