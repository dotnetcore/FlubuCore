using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FlubuCore.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if NETCOREAPP3_1
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
 #else
            var host = new WebHostBuilder()
                .UseKestrel(o =>
                {
                    o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
                })

                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
#endif
        }
    }
}
