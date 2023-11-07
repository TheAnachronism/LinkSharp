using FastEndpoints;
using FluentResults;
using LinkSharp.Database;
using LinkSharp.Errors;
using LinkSharp.Extensions;
using Microsoft.AspNetCore.Identity;

namespace LinkSharp.Features.Authentication.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Result> ExecuteAsync(RegisterCommand command, CancellationToken ct)
    {
        var user = await _userManager.FindByNameAsync(command.Username) ?? await _userManager.FindByEmailAsync(command.Email);
        if (user is not null)
            return Result.Fail(new DuplicateEntryError<ApplicationUser>(user, "A user with this username or email already exists!"));

        user = new ApplicationUser
        {
            Email = command.Email,
            UserName = command.Username
        };

        var identityResult = await _userManager.CreateAsync(user, command.Password);
        if (!identityResult.Succeeded)
            return identityResult.MapToFluentResultFail();

        await _signInManager.SignInAsync(user, true);
        return Result.Ok();
    }
}