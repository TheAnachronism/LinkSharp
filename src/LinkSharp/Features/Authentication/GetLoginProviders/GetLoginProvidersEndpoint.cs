using FastEndpoints;
using LinkSharp.Database;
using Microsoft.AspNetCore.Identity;

namespace LinkSharp.Features.Authentication.GetLoginProviders;

public record GetLoginProviderResponse(string ProviderName);

public class GetLoginProvidersEndpoint : EndpointWithoutRequest<IEnumerable<GetLoginProviderResponse>>
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public GetLoginProvidersEndpoint(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("/api/auth/GetLoginProviders");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var providers = await _signInManager.GetExternalAuthenticationSchemesAsync();
        await SendOkAsync(providers.Select(x => new GetLoginProviderResponse(x.Name)), ct);
    }
}

