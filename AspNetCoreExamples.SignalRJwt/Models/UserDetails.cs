using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreExamples.SignalRJwt.Models
{
    public class UserDetails
    {
        public UserDetails(string connectionId, string name, int id)
        {
            ConnectionId = connectionId;
            Name = name;
            Id = id;
        }

        public string ConnectionId { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }
    }
}
