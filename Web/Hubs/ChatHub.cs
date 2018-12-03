using Interfaces;
using Microsoft.AspNetCore.SignalR;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Web.Hubs
{
    public class ChatHub : Hub
    {
        public ChatHub(IClusterClient client)
        {
            _client = client;
        }

        private readonly IClusterClient _client;

        private string GetFakeLogin()
        {
            if (Context.GetHttpContext().Request.Cookies.TryGetValue("FakeLogin", out var fakeLogin) && !string.IsNullOrWhiteSpace(fakeLogin))
            {
                return fakeLogin;
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        public Task SendMessageAsync(Guid channelId, string message)
        {
            return _client.GetGrain<ISignalRConnectionGrain>(Guid.Parse(Context.ConnectionId))
                .SendMessageAsync(channelId, GetFakeLogin(), message);
        }
    }
}
