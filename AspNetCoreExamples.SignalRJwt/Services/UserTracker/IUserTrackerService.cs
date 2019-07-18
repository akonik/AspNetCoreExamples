using AspNetCoreExamples.SignalRJwt.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreExamples.SignalRJwt.Services.UserTracker
{
    public interface IUserTrackerService
    {
        Task AddUser(HubConnectionContext connection, UserDetails details);
        Task<bool> AreUserOnline(HubConnectionContext connection);
        bool AreUserOnline(int userId);
        Task<IEnumerable<UserDetails>> GetConnectedUsers();
        Task RemoveUser(HubConnectionContext connection);

        Task<UserDetails> GetUserById(int id);

        Task<IEnumerable<UserDetails>> GetUsersById(params int[] ids);

        event Action<UserDetails> UserJoined;
        event Action<UserDetails> UserLeft;
    }
}
