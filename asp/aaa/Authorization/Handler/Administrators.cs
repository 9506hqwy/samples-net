using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Aaa.Authorization.Handler;

#pragma warning disable CA1812
internal sealed class Administrators : AuthorizationHandler<Requirement.Administrators>
#pragma warning restore CA1812
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        Requirement.Administrators requirement)
    {
        if (context.User.HasClaim(
            c => c.Type == ClaimTypes.Name &&
            requirement.Accounts.Contains(c.Value)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

