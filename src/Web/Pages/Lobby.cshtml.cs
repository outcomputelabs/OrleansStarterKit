using Grains;
using Grains.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orleans;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        /// <summary>
        /// Details of the signed-in account.
        /// </summary>
        public AccountInfo CurrentAccount { get; set; }

        /// <summary>
        /// Account information to display for selection.
        /// </summary>
        public IEnumerable<AccountInfo> Accounts { get; set; }

        /// <summary>
        /// List of handles that the current account is following.
        /// </summary>
        public HashSet<string> Following { get; set; }

        [Required]
        [StringLength(100)]
        [FromForm]
        public string FollowTarget { get; set; }

        [FromForm]
        public bool DoFollow { get; set; }

        [FromForm]
        public bool DoUnfollow { get; set; }

        #endregion

        public LobbyModel(IClusterClient client)
        {
            _client = client;
        }

        public async Task OnGetAsync()
        {
            // get current account information from orleans
            CurrentAccount = await _client.GetGrain<IAccount>(User.Identity.Name.ToLowerInvariant()).GetInfoAsync();

            // get the lobby account information
            Accounts = await _client.GetGrain<ILobby>(Guid.Empty).GetAccountInfoListAsync();

            // get the list of accounts being followed
            Following = (await _client.GetGrain<IAccount>(User.Identity.Name.ToLowerInvariant()).GetFollowingAsync())
                .Select(x => x.UniformHandle)
                .ToHashSet();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // validate the input model
            if (!ModelState.IsValid)
            {
                return RedirectToPage();
            }

            // access the target account
            var target = _client.GetGrain<IAccount>(FollowTarget);
            var info = await target.GetInfoAsync();

            // check the type of action
            if (DoFollow)
            {
                // attempt to follow the target account
                await _client.GetGrain<IAccount>(User.Identity.Name.ToLowerInvariant()).FollowAsync(info, target);
            }
            else if (DoUnfollow)
            {
                // attempt to unfollow the target account

            }

            // refresh the page
            return RedirectToPage();
        }
    }
}