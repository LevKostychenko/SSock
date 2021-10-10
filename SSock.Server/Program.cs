using SSock.Core.Infrastructure;
using SSock.Server.Extensions;

namespace SSock.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBuilder
                .BuildServiceCollection()
                .AddDI();
        }
    }
}