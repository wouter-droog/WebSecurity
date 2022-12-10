using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using WebAppIdentity.Data.Account;

namespace WebAppIdentity.Pages.Account
{
    [Authorize]
    public class AuthenticatorSetup : PageModel
    {
        private readonly UserManager<User> _userManager;
        
        [BindProperty]
        public string? AuthenticatorKey { get; set; }

        public AuthenticatorSetup(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            // Get the user who is currently logged in.
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Generate the key
            // Microsoft Authenticator app.
            await _userManager.ResetAuthenticatorKeyAsync(user);
            AuthenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user) ?? string.Empty;
            
            // if AuthenticatorKey is null or empty, add error to model state
            if (string.IsNullOrEmpty(AuthenticatorKey))
            {
                ModelState.AddModelError(string.Empty, "Error generating authenticator key.");
            }
        }

        public async Task<IActionResult> OnPostAsync(string code)
        {
            // Get the user who is currently logged in.
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Verify the code entered by the user.
            var isCodeValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code);
            if (!isCodeValid)
            {
                ModelState.AddModelError("code", "Invalid code.");
                return Page();
            }
            
            // Enable two-factor authentication for the user.
            await _userManager.SetTwoFactorEnabledAsync(user, true);
            
            // Create success message to display on the page in temp data
            TempData["StatusMessage"] = "Two-factor authentication has been enabled. You can now use an authenticator app to generate verification codes when you log in.";
            

            // If the code is valid, the user's two-factor authentication setup
            // is complete. You can redirect the user to another page or show
            // a confirmation message.
            return RedirectToPage("./LoginTwoFactorAuthenticator");
        }
    }
}