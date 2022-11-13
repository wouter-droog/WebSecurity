using Microsoft.AspNetCore.Authorization;

namespace WebSecurity.Authorization;

public class ProbationRequirement : IAuthorizationRequirement
{
    public ProbationRequirement(int probationDays)
    {
        ProbationDays = probationDays;
    }

    public int ProbationDays { get; }
}