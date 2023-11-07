using FastEndpoints;
using FluentResults;
using LinkSharp.Database;
using LinkSharp.Errors;
using Microsoft.AspNetCore.Identity;

namespace LinkSharp.Features.Authentication.GetUserInfo;

public class GetUserInfoCommandHandler : ICommandHandler<GetUserInfoCommand, Result<ApplicationUser>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserInfoCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<ApplicationUser>> ExecuteAsync(GetUserInfoCommand command, CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(command.ClaimsPrincipal);
        return user is null
            ? Result.Fail(new UnauthorizedError())
            : Result.Ok(user);
    }
}