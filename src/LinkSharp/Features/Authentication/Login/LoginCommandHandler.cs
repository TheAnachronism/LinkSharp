using FastEndpoints;
using FluentResults;
using LinkSharp.Database;
using LinkSharp.Errors;
using Microsoft.AspNetCore.Identity;

namespace LinkSharp.Features.Authentication.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LoginCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Result> ExecuteAsync(LoginCommand command, CancellationToken ct)
    {
        var user = await _userManager.FindByNameAsync(command.UsernameOrEmail) ??
                   await _userManager.FindByEmailAsync(command.UsernameOrEmail);
        if (user is null)
            return Result.Fail(new UnauthorizedError());

        if (!await _userManager.CheckPasswordAsync(user, command.Password))
            return Result.Fail(new UnauthorizedError());

        await _signInManager.SignInAsync(user, true);
        return Result.Ok();
    }
}