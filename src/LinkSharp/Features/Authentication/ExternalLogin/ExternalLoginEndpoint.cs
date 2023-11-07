using System.Text.Encodings.Web;
using FastEndpoints;
using FluentValidation;
using LinkSharp.Database;
using Microsoft.AspNetCore.Identity;

namespace LinkSharp.Features.Authentication.ExternalLogin;

public class ExternalLoginRequest
{
    public string Provider { get; set; } = null!;
    public string ReturnUrl { get; set; } = "/";
}

public class ExternalLoginRequestValidator : Validator<ExternalLoginRequest>
{
    public ExternalLoginRequestValidator()
    {
        RuleFor(x => x.Provider).NotEmpty();
    }
}

public class ExternalLoginEndpoint : Endpoint<ExternalLoginRequest>
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ExternalLoginEndpoint(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("/api/auth/external-login/{Provider}");
    }

    public override Task HandleAsync(ExternalLoginRequest req, CancellationToken ct)
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(req.Provider,
            $"/api/auth/oidc-callback/{req.Provider}?returnUrl={UrlEncoder.Default.Encode(req.ReturnUrl)}");

        return SendResultAsync(Results.Challenge(properties, new List<string> { req.Provider }));
    }
}