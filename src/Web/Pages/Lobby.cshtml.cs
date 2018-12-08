using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Pages.Lobby
{
    [Authorize]
    public class LobbyModel : PageModel
    {
        private readonly IClusterClient _client;

        public class ChannelModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public IList<ChannelModel> Channels { get; set; }

        public LobbyModel(IClusterClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Channels = (await _client.GetGrain<ILobbyGrain>(Guid.Empty).GetChannels())
                .OrderBy(c => c.Name)
                .Select(c => new ChannelModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();

            return Page();
        }
    }
}