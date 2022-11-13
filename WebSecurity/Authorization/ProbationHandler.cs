using Microsoft.AspNetCore.Authorization;

namespace WebSecurity.Authorization;

public class ProbationHandler : AuthorizationHandler<ProbationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ProbationRequirement requirement)
    {
        var employmentDate = context.User.FindFirst(c => c.Type == Constants.EmploymentDateClaimType)?.Value;

        var isDate = DateTime.TryParse(employmentDate, out var date);

        if (!isDate) return Task.CompletedTask;

        var probationPeriod = DateTime.UtcNow - date;
        if (probationPeriod.Days > requirement.ProbationDays) context.Succeed(requirement);

        return Task.CompletedTask;
    }
}