using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppIdentity.Data.Account;

namespace WebAppIdentity.Pages.Account;

public class ConfirmEmail : PageModel
{
    private readonly UserManager<User> _userManager;
    
    [BindProperty] 
    public string? Message { get; set; }
    
    public ConfirmEmail(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    // method to check if token from email is valid
    public async Task<IActionResult> OnGetAsync(string token, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            Message = "Failed to verify email";
            return NotFound($"Unable to load user with email '{email}'.");
        }
        
        // check if token is valid: it will use same hashing algorithm as the one used to create the token (with the same key) and same user info
        // if the token then is the same as the one in the database, it means that the token is valid
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Error confirming email for user with ID '{user.Id}':");
        }
        
        Message = "Email confirmed successfully, you can now login";
        return Page();
    }
}