using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public ChatHub(IClusterClient client)
        {
            _client = client;
        }

        private readonly IClusterClient _client;

        public Task SendMessageAsync(Guid channelId, string message)
        {
            return _client.GetGrain<ISignalRConnectionGrain>(Guid.Parse(Context.ConnectionId))
                .SendMessageAsync(channelId, Context.User.Identity.Name, message);
        }
    }
}
