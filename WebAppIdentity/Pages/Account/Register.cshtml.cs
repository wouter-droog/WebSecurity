using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppIdentity.Services;

namespace WebAppIdentity.Pages.Account;

public class Register : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IEmailService _emailService;
    
    public Register(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
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
            // generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            
            //return Redirect(Url.PageLink("/Account/ConfirmEmail", values: new {email = user.Email, token}) ?? "Index");

            // generate confirmation link
            var confirmationLink = Url.Page("/Account/ConfirmEmail", null, new {email = user.Email, token}, Request.Scheme);
            
            // send confirmation email
            await _emailService.Send(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

            return RedirectToPage("Login");
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