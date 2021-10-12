using Microsoft.Extensions.DependencyInjection;
using SSock.Client.Core;
using SSock.Client.Core.Abstract.Clients;
using SSock.Client.Extensions;
using SSock.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SSock.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = ServiceBuilder
               .BuildServiceCollection()
               .AddConfiguration(
                    Path.Combine(
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                        @"Config\clientconfig.json"), args)
               .AddDI()
               .BuildServiceProvider();

            RunClientsAsync(provider).Wait();
        }

        static async Task RunClientsAsync(IServiceProvider services)
        {
            var clients = (IEnumerable<IClient>)services.GetServices(typeof(IClient));

            foreach (var client in clients)
            {
                await client.RunAsync();
            }
        }
    }
}
