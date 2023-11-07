using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace LinkSharp.Configuration;

public static class AuthenticationConfiguration
{
    public static AuthenticationBuilder ConfigureOpenIdConnectAuth(this AuthenticationBuilder authenticationBuilder, IConfiguration configuration)
    {
        authenticationBuilder.Services.AddOptions<AuthenticationSettings>()
            .Bind(configuration.GetSection("Authentication"))
            .ValidateDataAnnotations()
            .ValidateOidcProviderSettings();

        var serviceProvider = authenticationBuilder.Services.BuildServiceProvider();

        var providers = serviceProvider.GetRequiredService<IOptions<AuthenticationSettings>>().Value.OidcProviders;

        foreach (var provider in providers)
        {
            authenticationBuilder.AddOpenIdConnect(provider.ProviderName,
                options => ConfigureOpenIdConnectOptions(options, provider));
        }
        
        return authenticationBuilder;
    }

    private static void ConfigureOpenIdConnectOptions(OpenIdConnectOptions options, OidcSettings provider)
    {
        options.SignInScheme = IdentityConstants.ExternalScheme;

        options.Authority = provider.Authority;
        options.ClientId = provider.ClientId;
        options.ClientSecret = provider.ClientSecret;
        options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
        options.ResponseMode = OpenIdConnectResponseMode.FormPost;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.CallbackPath = $"/api/auth/external-callback/oidc/{provider.ProviderName}";

        options.ClaimsIssuer = provider.Authority;
        options.SaveTokens = true;

        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProviderForSignOut = context =>
            {
                var logoutUri = options.Configuration!.EndSessionEndpoint;

                var postLogoutUri = context.Properties.RedirectUri;
                if (!string.IsNullOrEmpty(postLogoutUri))
                {
                    if (postLogoutUri.StartsWith("/"))
                        postLogoutUri =
                            $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}{postLogoutUri}";

                    logoutUri += $"&post_logout_redirect_uri={Uri.EscapeDataString(postLogoutUri)}";
                }

                context.Response.Redirect(logoutUri);
                context.HandleResponse();

                return Task.CompletedTask;
            }
        };
    }

    private static OptionsBuilder<AuthenticationSettings> ValidateOidcProviderSettings(this OptionsBuilder<AuthenticationSettings> optionsBuilder)
    {
        optionsBuilder.Validate(config =>
        {
            return config.OidcProviders.DistinctBy(x => x.ProviderName).Count() == config.OidcProviders.Count;
        }, "Duplicate OIDC provider names!");
        
        return optionsBuilder;
    }
}