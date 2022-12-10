using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using WebAppIdentity.Data.Account;

namespace WebAppIdentity.Pages.Account
{
    public class LoginTwoFactorAuthenticator : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        public LoginTwoFactorAuthenticator(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }
        
        // GET
        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string code)
        {
            // Asp.NET will no which user is trying to login because of the cookie identity.TwoFactorUserId that was stored after the user entered the username and password
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(code, true, false);
            if (result.Succeeded != false) return RedirectToPage("/Index");
            
            // model error code is not valid
            ModelState.AddModelError("code", "Invalid code.");
            return Page();
        }
    }
}