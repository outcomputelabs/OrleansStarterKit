using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orleans;

namespace Web.Pages
{
    public class ChannelModel : PageModel
    {
        private readonly IClusterClient _client;

        public string ChannelName { get; set; }

        public ChannelModel(IClusterClient client)
        {
            _client = client;
        }

        public IActionResult OnGet()
        {
            return Page();
        }
    }
}