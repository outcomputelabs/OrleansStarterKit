using Grains;
using Grains.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orleans;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Pages
{
    public class SignInModel : PageModel
    {
        #region ViewModel

        [Required]
        [StringLength(100)]
        [Display(Name = "Handle")]
        [RegularExpression(@"^[a-zA-Z]{1}[a-z0-9]*$")]
        [BindProperty]
        [FromForm]
        public string Handle { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Display Name")]
        [RegularExpression(@"^\w+(\s\w+)*")]
        [BindProperty]
        [FromForm]
        public string DisplayName { get; set; }

        #endregion

        private readonly IClusterClient _client;

        public SignInModel(IClusterClient client)
        {
            _client = client;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // sign in users with whatever information they provide
            // this is to facilitate the use of this app in a live demo scenario
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Handle)
            };
            var identity = new ClaimsIdentity(claims, "login");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal, new AuthenticationProperties { IsPersistent = true });

            // also save this user information to orleans
            await _client.GetGrain<IUser>(Handle.ToLowerInvariant()).SetInfoAsync(new UserInfo(Handle, DisplayName));

            return RedirectToPage("/lobby");
        }
    }
}