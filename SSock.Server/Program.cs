using Microsoft.Extensions.DependencyInjection;
using SSock.Core.Infrastructure;
using SSock.Server.Core.Abstract.ServerEngine;
using SSock.Server.Extensions;
using System.IO;
using System.Reflection;

namespace SSock.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = ServiceBuilder
                .BuildServiceCollection()
                .AddConfiguration(
                    Path.Combine(
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                        @"Config\serverconfig.json"), args)
               .AddDI()
               .BuildServiceProvider();

            var runner = (IServerRunner)services.GetService(typeof(IServerRunner));

            runner.Run();
        }
    }
}