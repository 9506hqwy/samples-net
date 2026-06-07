using Microsoft.AspNetCore.Authorization;

namespace Aaa.Authorization.Requirement;

internal sealed class Administrators : IAuthorizationRequirement
{
#pragma warning disable CA1822
    internal string[] Accounts =>
#pragma warning restore CA1822
    [
        "root",
        "admin"
    ];
}
