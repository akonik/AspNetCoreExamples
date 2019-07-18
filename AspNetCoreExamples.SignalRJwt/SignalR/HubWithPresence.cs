using AspNetCoreExamples.SignalRJwt.Models;
using AspNetCoreExamples.SignalRJwt.Services.UserTracker;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreExamples.SignalRJwt.SignalR
{
    public class HubWithPresence : Hub
    {
        private IUserTrackerService _userTracker;

        public HubWithPresence(IUserTrackerService userTracker)
        {
            _userTracker = userTracker;
        }

        public Task<IEnumerable<UserDetails>> GetUsersOnline()
        {
            return _userTracker.GetConnectedUsers();
        }

        public virtual Task OnUserConnected(UserDetails user)
        {
            return Task.CompletedTask;
        }
    }
}
