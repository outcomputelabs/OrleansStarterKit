using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orleans;
using System.Collections.Generic;

namespace Web.Pages
{
    [Authorize]
    public class LobbyModel : PageModel
    {
        private readonly IClusterClient _client;

        public IList<Grains.Models.ChannelModel> Channels { get; set; }

        public LobbyModel(IClusterClient client)
        {
            _client = client;
        }

        public IActionResult OnGet()
        {
            /*
            Channels = (await _client.GetGrain<ILobby>(Guid.Empty).GetChannels())
                .OrderBy(c => c.Name)
                .ToList();
            */

            return Page();
        }
    }
}