using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppIdentity.Data.Account;
using WebAppIdentity.Services;

namespace WebAppIdentity.Pages.Account;

public class LoginWith2fa : PageModel
{
    // BindProperty for the input model
    [BindProperty]
    public LoginWith2faModel Input { get; set; }
    
    // field with user manager and constructor
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailService _emailService;
    
    public LoginWith2fa(UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    // async get method with input email and remember me boolean that will be used to login with 2fa
    public async Task<IActionResult> OnGetAsync(string email, bool rememberMe)
    {
        // set email to view data
        ViewData["Email"] = email;
        
        Input = new LoginWith2faModel
        {
            Code = string.Empty,
            RememberMe = rememberMe
        };

        // generate 2fa code and send it to user by email
        await Generate2faCode(email);

        // return page
        return Page();
    }
    
    // async post method with input email and remember me boolean that will be used to login with 2fa
    public async Task<IActionResult> OnPostAsync(string email, bool rememberMe)
    {
        // if model is not valid return page
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // get user by email
        var user = await _userManager.FindByEmailAsync(email);

        // if user is null return page
        if (user == null)
        {
            return Page();
        }

        // sign in user with 2fa email
        var result = await _signInManager.TwoFactorSignInAsync("Email",Input.Code, rememberMe, false);    
        
        // if result is succeeded redirect to index page
        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }

        // if result is locked out redirect to lockout page
        if (result.IsLockedOut)
        {
            return RedirectToPage("./Lockout");
        }

        // if result is not succeeded add error to model state
        ModelState.AddModelError(string.Empty, "Invalid authenticator code.");

        // return page
        return Page();
    }

    private async Task Generate2faCode(string email)
    {
        // get user by email
        var user = await _userManager.FindByEmailAsync(email);
        
        // generate 2fa code
        var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
        
        // add 2fa code to cookies
        Response.Cookies.Append("2faCode", code);
        
        // send email to user with 2fa code
        await _emailService.Send(email, "2fa code", code);
    }
}

public class LoginWith2faModel
{
    [Required]
    [Display(Name = "Security Code")]
    public string Code { get; set; }
    
    public bool RememberMe { get; set; }
}