namespace WebSecurity.Pages;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Constants.HRClaimPolicy)]
public class HumanResource : PageModel
{
    public void OnGet()
    {
    }
}