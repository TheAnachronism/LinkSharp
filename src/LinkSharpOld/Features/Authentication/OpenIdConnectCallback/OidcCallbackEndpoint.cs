using FastEndpoints;
using FluentResults;
using FluentValidation;
using LinkSharp.Database;
using LinkSharp.Errors;

namespace LinkSharp.Features.Authentication.OpenIdConnectCallback;

public class CallbackRequest
{
    public string Provider { get; set; } = null!;
    public string ReturnUrl { get; set; } = "/";
}

public class CallbackRequestValidator : Validator<CallbackRequest>
{
    public CallbackRequestValidator()
    {
        RuleFor(x => x.Provider).NotEmpty();
    }
}

public class OidCallbackCommand : ICommand<Result>
{
}

public class OidcCallbackEndpoint : Endpoint<CallbackRequest>
{
    public override void Configure()
    {
        AllowAnonymous();
        Get("/api/auth/oidc-callback/{Provider}");
    }

    public override async Task HandleAsync(CallbackRequest req, CancellationToken ct)
    {
        var result = await new OidCallbackCommand().ExecuteAsync(ct);

        if (result.IsSuccess)
            await SendRedirectAsync(req.ReturnUrl);
        else if (result.HasError<UnauthorizedError>())
            await SendUnauthorizedAsync();
        else if (result.HasError<DuplicateEntryError<ApplicationUser>>())
        {
            AddError("Internal user already exists, requires linking!");
            await SendErrorsAsync();
        }
        else
            await SendResultAsync(Results.Conflict());
    }
}