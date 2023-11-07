using FluentResults;
using LinkSharp.Database;
using LinkSharp.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LinkSharp.Pages.Authentication.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result>
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginCommandHandler(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.UsernameOrEmail) ??
                   await _userManager.FindByEmailAsync(request.UsernameOrEmail);

        if (user is null)
            return new UnauthorizedError();

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return new UnauthorizedError();
        
        await _signInManager.SignInAsync(user, true);
        return Result.Ok();
    }
}