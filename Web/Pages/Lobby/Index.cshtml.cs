using Interfaces;
using Microsoft.AspNetCore.Mvc;
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

        public class ChannelModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public IList<ChannelModel> Channels { get; set; }

        public IndexModel(IClusterClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (Request.Cookies.TryGetValue("FakeLogin", out var cookie) && !string.IsNullOrWhiteSpace(cookie))
            {
                FakeLogin = cookie;
            }
            else
            {
                return RedirectToPage("/FakeLogin");
            }

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