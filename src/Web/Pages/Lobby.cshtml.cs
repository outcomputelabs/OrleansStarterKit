using Grains;
using Grains.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orleans;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Web.Pages
{
    [Authorize]
    public class LobbyModel : PageModel
    {
        #region Dependencies

        private readonly IClusterClient _client;

        #endregion

        #region ViewModel

        public UserInfo UserInfo { get; set; }

        public IEnumerable<ChannelInfo> Channels { get; set; }

        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9]{1}[a-zA-Z0-9\-]+$")]
        [FromForm]
        [BindProperty]
        public string NewChannelName { get; set; }

        #endregion

        public LobbyModel(IClusterClient client)
        {
            _client = client;
        }

        public async Task OnGetAsync()
        {
            // get current user information from orleans
            UserInfo = await _client.GetGrain<IUser>(User.Identity.Name.ToLowerInvariant()).GetInfoAsync();

            // get the list of channels from orleans
            Channels = await _client.GetGrain<ILobby>(Guid.Empty).GetChannelsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // get current user information from orleans
            UserInfo = await _client.GetGrain<IUser>(User.Identity.Name.ToLowerInvariant()).GetInfoAsync();

            // validate input
            if (!ModelState.IsValid) return Page();

            // attempt to create the new channel
            await _client.GetGrain<ILobby>(Guid.Empty).CreateChannelAsync(new ChannelInfo(NewChannelName, DateTime.UtcNow));

            // done
            return RedirectToPage("/Channel", new
            {
                Name = NewChannelName
            });
        }
    }
}