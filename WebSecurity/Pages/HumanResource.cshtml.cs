using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebSecurity.Pages;

[Authorize(Policy = Constants.HRClaimPolicy)]
public class HumanResource : PageModel
{
    public void OnGet()
    {
    }
}