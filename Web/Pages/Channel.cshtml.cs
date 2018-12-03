using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orleans;
using System.Threading.Tasks;

namespace Web.Pages
{
    public class ChannelModel : PageModel
    {
        private readonly IClusterClient _client;

        public string FakeLogin { get; set; }

        public ChannelModel(IClusterClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            #region Fake Login

            if (Request.Cookies.TryGetValue("FakeLogin", out var cookie) && !string.IsNullOrWhiteSpace(cookie))
            {
                FakeLogin = cookie;
            }
            else
            {
                return RedirectToPage("/FakeLogin");
            }

            #endregion

            return Page();
        }
    }
}