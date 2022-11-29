using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppIdentity.Pages.Account;

public class Register : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    
    public Register(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [BindProperty]
    public RegisterViewModel RegisterViewModel { get; set; }
    
    public void OnGet()
    {
        
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        // check if model is valid
        if (!ModelState.IsValid) return Page();
        
        // create user
        var user = new IdentityUser
        {
            UserName = RegisterViewModel.Email,
            Email = RegisterViewModel.Email
        };
            
        // create user in database
        var result = await _userManager.CreateAsync(user, RegisterViewModel.Password);
            
        // check if user is created successfully
        if (result.Succeeded)
        {
            // // sign in user
            // await _signInManager.SignInAsync(user, isPersistent: false);
            //     
            // // redirect to home page
            // return RedirectToPage("/Index");
            
            return RedirectToPage("Account/Login");
        }
            
        // add errors to model state
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("Register", error.Description);
        }

        // if we got this far, something failed, redisplay form
        return Page();
    }
}

// Create a register view model to hold the user's input
public class RegisterViewModel
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}