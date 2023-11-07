using FluentResults;
using LinkSharp.Database;
using LinkSharp.Errors;
using LinkSharp.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LinkSharp.Pages.Authentication.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username) ?? await _userManager.FindByEmailAsync(request.Email);
        if (user is not null)
            return new DuplicateEntryError<ApplicationUser>(user, "A user with this username or email already exists!");

        user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Username
        };

        var identityResult = await _userManager.CreateAsync(user, request.Password);
        if (!identityResult.Succeeded)
            return identityResult.MapToFluentResultFail();

        await _signInManager.SignInAsync(user, true);
        return Result.Ok();
    }
}