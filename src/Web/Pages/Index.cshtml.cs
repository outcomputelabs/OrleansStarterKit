using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        public ActionResult OnGet()
        {
            return RedirectToPage("signin");
        }
    }
}
