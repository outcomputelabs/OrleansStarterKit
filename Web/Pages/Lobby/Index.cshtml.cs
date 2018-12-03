using Interfaces;
using Interfaces.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Pages.Lobby
{
    public class IndexModel : PageModel
    {
        private readonly IClusterClient _client;

        public string FakeLogin { get; set; }

        public IList<Channel> Channels { get; set; }

        public IndexModel(IClusterClient client)
        {
            _client = client;
        }

        public async Task OnGetAsync()
        {
            if (Request.Cookies.TryGetValue("FakeLogin", out var cookie))
            {
                FakeLogin = cookie;
            }
            else
            {
                RedirectToPage("/FakeLogin");
            }

            Channels = (await _client.GetGrain<ILobbyGrain>(Guid.Empty).GetChannels())
                .OrderBy(c => c.Name)
                .ToList();
        }
    }
}