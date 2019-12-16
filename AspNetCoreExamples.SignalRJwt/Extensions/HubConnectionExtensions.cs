using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCoreExamples.SignalRJwt.Extensions
{
    public static class HubConnectionExtensions
    {
        public static int? UserId(this HubConnectionContext context)
        {
            var sub = context.GetHttpContext().User?.Claims?.SingleOrDefault(x => x.Type == "id")?.Value;
            return !string.IsNullOrEmpty(sub) ? new int?(int.Parse(sub)) : null;
        }

        public static string UserName(this HubConnectionContext context)
        {
            return context.User.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}
