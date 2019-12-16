using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreExamples.SignalRJwt.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToUnixEpochDate(this DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalMilliseconds);

        public static long? ToUnixEpochDate(this DateTime? date)
        {
            if (date == null)
                return null;

            return (long)Math.Round((date.Value.ToUniversalTime() -
                              new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                             .TotalMilliseconds);
        }

    }
}
