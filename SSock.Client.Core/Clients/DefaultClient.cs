using Microsoft.Extensions.Configuration;
using SSock.Client.Core.Abstract.Clients;
using System;
using System.Threading.Tasks;

namespace SSock.Client.Core.Clients
{
    internal class DefaultClient
        : BaseClient,
        IClient
    {
        public DefaultClient(IConfiguration configuration)
            : base(configuration.GetSection("server"))
        {}

        public override async Task RunAsync()
        {
            await base.RunAsync();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
