using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orleans;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Pages
{
    [Authorize]
    public class MyFeedModel : PageModel
    {
        #region Setup

        private readonly IClusterClient _client;

        public MyFeedModel(IClusterClient client)
        {
            _client = client;
        }

        #endregion

        #region ViewModel

        public IEnumerable<Message> Messages { get; set; }

        #endregion

        public async Task OnGetAsync()
        {
            // get latest feed items from current user
            Messages = (await _client.GetGrain<IFeed>(User.Identity.Name).GetLatestMessagesAsync())
                .OrderByDescending(x => x.Timestamp);
        }
    }
}