using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Orleans;
using System.Threading.Tasks;

namespace Web.Hubs
{
    [Authorize]
    public class FeedHub : Hub
    {
        public FeedHub(IClusterClient client)
        {
            _client = client;
        }

        private readonly IClusterClient _client;

        public Task PublishMessageAsync(string content)
        {
            return _client.GetGrain<IFeed>(Context.User.Identity.Name).PublishMessageAsync(content);
        }
    }
}
