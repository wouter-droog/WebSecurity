namespace WebSecurity.Pages.Account;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class Login : PageModel
{
    [BindProperty] public Credential? Credential { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (ModelState.IsValid)
        {
            if (Credential?.UserName == "admin" && Credential.Password == "admin")
            {
                return RedirectToPage("/Index");
            }

            ModelState.AddModelError("Credential.Username", "Invalid username or password");
            return Page();
        }

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