using AspNetCoreExamples.SignalRJwt.Extensions;
using AspNetCoreExamples.SignalRJwt.Models;
using AspNetCoreExamples.SignalRJwt.Services.UserTracker;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCoreExamples.SignalRJwt.SignalR
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : HubWithPresence
    {
        private readonly IUserTrackerService _userTracker;

        public ChatHub(IUserTrackerService userTracker)
            : base(userTracker)
        {
            _userTracker = userTracker ?? throw new ArgumentNullException(nameof(userTracker));
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public async override Task OnUserConnected(UserDetails user)
        {
            await Clients.Client(user.ConnectionId)
                .SendAsync("OnUserJoined", $"Welcome {user.Name}");
        }


        public async Task Say(string message)
        {
            await Clients.All.SendAsync("OnMessageReceived",
                new
                {
                    user = new
                    {
                        id = UserId,
                        Name = UserName
                    },
                    message,
                    date = DateTime.UtcNow.ToUnixEpochDate()
                });
        }

        protected int? UserId
        {
            get
            {
                var sub = Context.GetHttpContext().User?.Claims?.SingleOrDefault(x => x.Type == "id")?.Value;
                return !string.IsNullOrEmpty(sub) ? new int?(int.Parse(sub)) : null;
            }
        }

        protected string UserName => Context.User.Claims.SingleOrDefault(
            x => x.Type == ClaimTypes.NameIdentifier).Value;
    }
}
