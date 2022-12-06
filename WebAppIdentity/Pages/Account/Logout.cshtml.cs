using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppIdentity.Data.Account;

namespace WebAppIdentity.Pages.Account;

public class Logout : PageModel
{
    private readonly SignInManager<User> _signInManager;
    
    public Logout(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        await _signInManager.SignOutAsync();

        return RedirectToPage("/Account/Login");
    }
}