using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebSecurity.Authorization;

namespace WebSecurity.Pages.Account;

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
                new(ClaimTypes.Name, Credential.UserName),
                new(ClaimTypes.Email, "admin@myWebsite.com"),
                new(ClaimTypes.Role, Constants.AdminRole),
                new(Constants.HRDepartmentClaimType, Constants.HRDepartmentClaimValue),
                new(Constants.EmploymentDateClaimType, "2022-01-01")
            };
            var identity = new ClaimsIdentity(claims, Constants.CookieScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = Credential.ShouldRememberMe
            };

            await HttpContext.SignInAsync("MyCookieAuthenticationScheme", principal, authProperties);

            return RedirectToPage("/Index");
        }

        ModelState.AddModelError("Credential.UserName", "Invalid username or password");
        ModelState.AddModelError("Credential.Password", "Invalid username or password");
        return Page();
    }
}
