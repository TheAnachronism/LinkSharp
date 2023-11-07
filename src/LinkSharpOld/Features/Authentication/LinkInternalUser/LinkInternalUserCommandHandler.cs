using FastEndpoints;
using FluentResults;
using LinkSharp.Database;
using LinkSharp.Errors;
using LinkSharp.Extensions;
using Microsoft.AspNetCore.Identity;

namespace LinkSharp.Features.Authentication.LinkInternalUser;

public class LinkInternalUserCommandHandler : ICommandHandler<LinkInternalUserCommand, Result>
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public LinkInternalUserCommandHandler(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<Result> ExecuteAsync(LinkInternalUserCommand command, CancellationToken ct)
    {
        var externalInfo = await _signInManager.GetExternalLoginInfoAsync();
        if (externalInfo is null)
            return Result.Fail(new UnauthorizedError());

        var user = await _userManager.FindByNameAsync(command.UsernameOrEmail) ??
                   await _userManager.FindByEmailAsync(command.UsernameOrEmail);

        if (user is null)
            return Result.Fail(new UnauthorizedError());

        if (!await _userManager.CheckPasswordAsync(user, command.Password))
            return Result.Fail(new UnauthorizedError());

        var result = await _userManager.AddLoginAsync(user, externalInfo);
        if (!result.Succeeded)
            return result.MapToFluentResultFail();

        await _userManager.UpdateAsync(user);
        await _signInManager.SignInAsync(user, true);
        return Result.Ok();
    }
}