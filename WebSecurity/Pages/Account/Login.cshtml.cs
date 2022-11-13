namespace WebSecurity.Pages.Account;

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class Login : PageModel
{
    [BindProperty] public Credential? Credential { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        if (Credential?.UserName == "admin" && Credential.Password == "password")
        {
            // Creating the security context
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Credential.UserName),
                new Claim(ClaimTypes.Email, "admin@myWebsite.com"),
                new Claim(ClaimTypes.Role, Constants.AdminRole),
                new Claim(Constants.HRDepartmentClaimType, Constants.HRDepartmentClaimValue)
            };
            var identity = new ClaimsIdentity(claims, Constants.CookieScheme);
            var principal = new ClaimsPrincipal(identity);
            
            await HttpContext.SignInAsync("MyCookieAuthenticationScheme", principal);
            
            return RedirectToPage("/Index");
        }

        ModelState.AddModelError("Credential.UserName", "Invalid username or password");
        ModelState.AddModelError("Credential.Password", "Invalid username or password");
        return Page();

    }
}

public class Credential
{
    [Required]
    [Display(Name = "User Name")]
    public string? UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string? Password { get; set; }
}