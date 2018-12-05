using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Pages
{
    public class SignInModel : PageModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Handle")]
        [RegularExpression("[a-z]{1}[a-z0-9]{0,99}")]
        [BindProperty]
        public string Handle { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Handle)
            };
            var identity = new ClaimsIdentity(claims, "login");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);

            return RedirectToPage("MyFeed");
        }
    }
}