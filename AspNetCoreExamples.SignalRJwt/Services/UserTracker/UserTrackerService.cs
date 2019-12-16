using AspNetCoreExamples.SignalRJwt.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreExamples.SignalRJwt.Services.UserTracker
{
    public class UserTrackerService : IUserTrackerService
    {
        private readonly ConcurrentDictionary<HubConnectionContext, UserDetails>
            _connectedUsers = new ConcurrentDictionary<HubConnectionContext, UserDetails>();

        public event Action<UserDetails> UserJoined;
        public event Action<UserDetails> UserLeft;

        public Task AddUser(HubConnectionContext connection, UserDetails details)
        {
            _connectedUsers.TryAdd(connection, details);
            UserJoined(details);
            return Task.CompletedTask;
        }

        public Task RemoveUser(HubConnectionContext connection)
        {
            _connectedUsers.Remove(connection, out var details);
            UserLeft(details);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<UserDetails>> GetConnectedUsers()
        {
            return Task.FromResult(_connectedUsers.Values.AsEnumerable());
        }

        public Task<bool> AreUserOnline(HubConnectionContext connection)
        {
            return Task.FromResult(_connectedUsers.ContainsKey(connection));
        }

        public Task<UserDetails> GetUserById(int id)
        {
            return Task.FromResult(_connectedUsers.SingleOrDefault(x => x.Value.Id == id).Value);
        }

        public Task<IEnumerable<UserDetails>> GetUsersById(params int[] ids)
        {
            return Task.FromResult(_connectedUsers.Where(x => ids.Contains(x.Value.Id)).Select(x => x.Value));
        }

        public bool AreUserOnline(int userId)
        {
            return _connectedUsers.Values.Any(x => x.Id == userId);
        }
    }
}
