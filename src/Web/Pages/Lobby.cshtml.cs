using Grains;
using Grains.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orleans;
using System;
using System.Collections.Generic;
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

        #endregion

        public LobbyModel(IClusterClient client)
        {
            _client = client;
        }

        public async Task OnGetAsync()
        {
            // get current user information from orleans
            UserInfo = await _client.GetGrain<IUser>(User.Identity.Name).GetInfoAsync();

            // get the list of channels from orleans
            Channels = await _client.GetGrain<ILobby>(Guid.Empty).GetChannelsAsync();
        }
    }
}