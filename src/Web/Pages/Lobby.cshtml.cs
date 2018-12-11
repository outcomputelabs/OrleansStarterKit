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

        public UserInfo CurrentUser { get; set; }

        /// <summary>
        /// User information to display for selection.
        /// </summary>
        public IEnumerable<UserInfo> Users { get; set; }

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
            CurrentUser = await _client.GetGrain<IUser>(User.Identity.Name.ToLowerInvariant()).GetInfoAsync();

            // get the lobby user information
            Users = await _client.GetGrain<ILobby>(Guid.Empty).GetUserInfoListAsync();
        }
    }
}