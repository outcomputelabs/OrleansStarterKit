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
        [Display(Name = "User Name")]
        [RegularExpression(@"^[a-zA-Z]{1}[a-z0-9]*$")]
        [BindProperty]
        [FromForm]
        public string UserName { get; set; }

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
            // this is to facilitate use of this app in a live demo scenario
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, UserName)
            };
            var identity = new ClaimsIdentity(claims, "login");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal, new AuthenticationProperties { IsPersistent = true });

            // also save this user on orleans
            await _client.GetGrain<IUser>(UserName.ToLowerInvariant()).SetInfoAsync(new UserInfo(UserName, DisplayName));

            return RedirectToPage("/lobby");
        }
    }
}