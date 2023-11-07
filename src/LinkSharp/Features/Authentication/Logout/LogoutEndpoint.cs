using FastEndpoints;
using LinkSharp.Database;
using Microsoft.AspNetCore.Identity;

namespace LinkSharp.Features.Authentication.Logout;

public class LogoutEndpoint : EndpointWithoutRequest
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LogoutEndpoint(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("/api/auth/logout");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await _signInManager.SignOutAsync();
        await SendOkAsync();
    }
}