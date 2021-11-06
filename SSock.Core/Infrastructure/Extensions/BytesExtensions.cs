using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSock.Core.Infrastructure.Extensions
{
    public static class BytesExtensions
    {
        public static string BytesToString(this IEnumerable<byte> bytes)
            => Encoding.Unicode.GetString(
                        bytes.ToArray(),
                        0,
                        bytes.Count());
    }
}
