using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Controllers;

[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    public IActionResult Authenticate([FromBody] Credentials credentials)
    {
        if (credentials.Username == "admin" && credentials.Password == "password")
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, credentials.Username),
                new Claim(ClaimTypes.Role, "Admin"),
                new(ClaimTypes.Email, "admin@myWebsite.com"),
                new("Department", "HR"),
                new("EmploymentDate", "2022-01-01")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("SecretKey")
                                                                      ?? throw new NullReferenceException(
                                                                          "SecretKey is null")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                "http://localhost:5000",
                "http://localhost:5000",
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(10),
                creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expires = token.ValidTo
            });
        }

        return Unauthorized("You are not authorized");
    }
}

public class Credentials
{
    public string Username { get; set; }
    public string Password { get; set; }
}