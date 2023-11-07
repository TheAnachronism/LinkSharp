using System.Security.Claims;
using FastEndpoints;
using FluentResults;
using LinkSharp.Database;

namespace LinkSharp.Features.Authentication.GetUserInfo;

public class GetUserInfoCommand : ICommand<Result<ApplicationUser>>
{
    public ClaimsPrincipal ClaimsPrincipal { get; }

    public GetUserInfoCommand(ClaimsPrincipal claimsPrincipal)
    {
        ClaimsPrincipal = claimsPrincipal;
    }
}