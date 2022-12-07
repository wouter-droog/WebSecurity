using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppIdentity.Data.Account;

namespace WebAppIdentity.Pages.Account;

[Authorize]
public class UserProfile : PageModel
{
    [BindProperty]
    public UserProfileViewModel UserProfileViewModel { get; set; }
    
    // bind property for success message
    [TempData]
    public string StatusMessage { get; set; }
    
    private readonly UserManager<User> _userManager;
    
    public UserProfile(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        var (user, departmentClaim, positionClaim) = await GetUserClaimsAsync();
        
        UserProfileViewModel = new UserProfileViewModel
        {
            Department = departmentClaim.Value,
            Position = positionClaim.Value,
            Email = user.Email ?? "Not set",
        };

        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var (user, departmentClaim, positionClaim) = await GetUserClaimsAsync();
            await _userManager.ReplaceClaimAsync(user, departmentClaim,  new Claim("Department", UserProfileViewModel.Department));
            await _userManager.ReplaceClaimAsync(user, positionClaim, new Claim("Position", UserProfileViewModel.Position));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            ModelState.AddModelError("UserProfile", "Unable to update user profile.");
        }
        
        StatusMessage = "Your profile has been updated";

        return RedirectToPage();
    }
    
    private async Task<(User user, Claim department, Claim position)> GetUserClaimsAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user is null)
        {
            throw new InvalidOperationException($"Unable to load user'.");
        }
        
        var claims = await _userManager.GetClaimsAsync(user);
        
        var department = claims.FirstOrDefault(c => c.Type == "Department") ?? throw new InvalidOperationException("Department claim not found");
        var position = claims.FirstOrDefault(c => c.Type == "Position") ?? throw new InvalidOperationException("Position claim not found");
        
        return (user, department, position);
    }
}

public class UserProfileViewModel
{
    public string Email { get; set; }
    
    [Required]
    public string Department { get; set; }
    
    [Required]
    public string Position { get; set; }
}