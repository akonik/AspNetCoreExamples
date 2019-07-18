using AspNetCoreExamples.SignalRJwt.Extensions;
using AspNetCoreExamples.SignalRJwt.Models;
using AspNetCoreExamples.SignalRJwt.Services.UserTracker;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreExamples.SignalRJwt.SignalR
{
    public class DefaultPresenceHublifetimeManager<THub> : PresenceHubLifetimeManager<THub, DefaultHubLifetimeManager<THub>>
        where THub : HubWithPresence
    {
        public DefaultPresenceHublifetimeManager(IUserTrackerService userTracker, IServiceScopeFactory serviceScopeFactory,
            ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
            : base(userTracker, serviceScopeFactory, loggerFactory, serviceProvider)
        {
        }
    }

    public class PresenceHubLifetimeManager<THub, THubLifetimeManager> : HubLifetimeManager<THub>, IDisposable
      where THubLifetimeManager : HubLifetimeManager<THub>
      where THub : HubWithPresence
    {
        private readonly HubConnectionList _connections = new HubConnectionList();
        private readonly IUserTrackerService _userTracker;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly HubLifetimeManager<THub> _wrappedHubLifetimeManager;
        private IHubContext<THub> _hubContext;

        public PresenceHubLifetimeManager(IUserTrackerService userTracker, IServiceScopeFactory serviceScopeFactory,
            ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            _userTracker = userTracker;

            _userTracker.UserJoined += _userTracker_UserJoined;
            _userTracker.UserLeft += _userTracker_UserUserLeft;

            _serviceScopeFactory = serviceScopeFactory;
            _serviceProvider = serviceProvider;
            _logger = loggerFactory.CreateLogger<PresenceHubLifetimeManager<THub, THubLifetimeManager>>();
            _wrappedHubLifetimeManager = serviceProvider.GetRequiredService<THubLifetimeManager>();
        }

        private async void _userTracker_UserUserLeft(UserDetails obj)
        {
            await Notify(hub => hub.OnUserConnected(obj));
        }

        private async void _userTracker_UserJoined(UserDetails obj)
        {
            await Notify(hub =>
            {
                if (obj.ConnectionId == hub.Context.ConnectionId)
                {
                    return hub.OnUserConnected(obj);
                }

                return Task.CompletedTask;
            });
        }

        public override async Task OnConnectedAsync(HubConnectionContext connection)
        {
            await _wrappedHubLifetimeManager.OnConnectedAsync(connection);
            _connections.Add(connection);
            var user = new UserDetails(connection.ConnectionId, connection.UserIdentifier, connection.UserId() ?? 0);
            await _userTracker.AddUser(connection, user);

        }

        public override async Task OnDisconnectedAsync(HubConnectionContext connection)
        {
            await _wrappedHubLifetimeManager.OnDisconnectedAsync(connection);
            _connections.Remove(connection);
            await _userTracker.RemoveUser(connection);
        }

        private async Task Notify(Func<THub, Task> invocation)
        {
            foreach (var connection in _connections)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var hubActivator = scope.ServiceProvider.GetRequiredService<IHubActivator<THub>>();
                    var hub = hubActivator.Create();

                    if (_hubContext == null)
                    {
                        // Cannot be injected due to circular dependency
                        _hubContext = _serviceProvider.GetRequiredService<IHubContext<THub>>();
                    }

                    hub.Clients = new HubCallerClients(_hubContext.Clients, connection.ConnectionId);
                    hub.Context = new DefaultHubCallerContext(connection);
                    hub.Groups = _hubContext.Groups;

                    try
                    {
                        await invocation(hub);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Presence notification failed.");
                    }
                    finally
                    {
                        hubActivator.Release(hub);
                    }
                }
            }
        }

        public void Dispose()
        {
            //_userTracker.UsersJoined -= OnUsersJoined;
            //_userTracker.UsersLeft -= OnUsersLeft;
        }


        public override Task SendAllAsync(string methodName, object[] args, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedHubLifetimeManager.SendAllAsync(methodName, args, cancellationToken);
        }

        public override Task SendAllExceptAsync(string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedHubLifetimeManager.SendAllExceptAsync(methodName, args, excludedConnectionIds, cancellationToken);
        }

        public override Task SendConnectionAsync(string connectionId, string methodName, object[] args, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedHubLifetimeManager.SendConnectionAsync(connectionId, methodName, args, cancellationToken);
        }

        public override Task SendConnectionsAsync(IReadOnlyList<string> connectionIds, string methodName, object[] args, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedHubLifetimeManager.SendConnectionsAsync(connectionIds, methodName, args, cancellationToken);
        }

        public override Task SendGroupAsync(string groupName, string methodName, object[] args, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedHubLifetimeManager.SendGroupAsync(groupName, methodName, args, cancellationToken);
        }

        public override Task SendGroupsAsync(IReadOnlyList<string> groupNames, string methodName, object[] args, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedHubLifetimeManager.SendGroupsAsync(groupNames, methodName, args, cancellationToken);
        }

        public override Task SendGroupExceptAsync(string groupName, string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedHubLifetimeManager.SendGroupExceptAsync(groupName, methodName, args, excludedConnectionIds, cancellationToken);
        }

        public override Task SendUserAsync(string userId, string methodName, object[] args, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedHubLifetimeManager.SendUserAsync(userId, methodName, args, cancellationToken);
        }

        public override Task SendUsersAsync(IReadOnlyList<string> userIds, string methodName, object[] args, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedHubLifetimeManager.SendUsersAsync(userIds, methodName, args, cancellationToken);
        }

        public override Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedHubLifetimeManager.AddToGroupAsync(connectionId, groupName, cancellationToken);
        }

        public override Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _wrappedHubLifetimeManager.RemoveFromGroupAsync(connectionId, groupName, cancellationToken);
        }
    }
}
