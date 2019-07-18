using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AspNetCoreExamples.SignalRJwt.SignalR
{

    public class HubConnectionList : IReadOnlyCollection<HubConnectionContext>
    {
        private readonly ConcurrentDictionary<string, HubConnectionContext> _connections = new ConcurrentDictionary<string, HubConnectionContext>();

        public HubConnectionContext this[string connectionId]
        {
            get
            {
                _connections.TryGetValue(connectionId, out var connection);
                return connection;
            }
        }

        public int Count => _connections.Count;

        public void Add(HubConnectionContext connection)
        {
            _connections.TryAdd(connection.ConnectionId, connection);
        }

        public void Remove(HubConnectionContext connection)
        {
            _connections.TryRemove(connection.ConnectionId, out _);
        }

        public IEnumerator<HubConnectionContext> GetEnumerator()
        {
            return _connections.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}