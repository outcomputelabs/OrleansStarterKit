using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orleans;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Web.Pages
{
    public class FakeLoginModel : PageModel
    {
        private readonly IClusterClient client;

        [Required]
        [StringLength(100)]
        [Display(Name = "User Name")]
        [BindProperty]
        public string UserName { get; set; }

        public bool IsFakeLoggedIn { get; set; }

        public string FakeLogin { get; set; }

        public FakeLoginModel(IClusterClient client)
        {
            this.client = client;
        }

        public void OnGet()
        {
            if (Request.Cookies.TryGetValue("FakeLogin", out var cookie))
            {
                IsFakeLoggedIn = true;
                FakeLogin = cookie;
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Response.Cookies.Append("FakeLogin", UserName);

            return RedirectToPage("./Lobby");
        }
    }
}