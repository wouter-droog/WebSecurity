using System.ComponentModel.DataAnnotations;

namespace WebSecurity.Authorization;

public class Credential
{
    [Required]
    [Display(Name = "User Name")]
    public string? UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string? Password { get; set; }

    [Display(Name = "Remember me?")] public bool ShouldRememberMe { get; set; }
}