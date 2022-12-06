using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppIdentity.Data.Account;

namespace WebAppIdentity.Pages.Account;

public class Login : PageModel
{
    private readonly SignInManager<User> _signInManager;

    public Login(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }
    
    [BindProperty]
    public LoginViewModel LoginViewModel { get; set; }

    
    public void OnGet()
    {
        
    }
    
    // async method to handle the login
    public async Task<IActionResult> OnPostAsync()
    {
        // check if the model is valid
        if (!ModelState.IsValid) return Page();
        
        var result = await _signInManager.PasswordSignInAsync(LoginViewModel.Email, LoginViewModel.Password, true, true);
        
        // check if the user is authenticated
        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError("Login", "User account locked out.");
            return Page();
        }
        
        // if the user is not authenticated, add an error to the model state
        ModelState.AddModelError("Login", "Invalid login attempt");

        // if we got this far, something failed, redisplay form
        return Page();
    }
}

public class LoginViewModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}