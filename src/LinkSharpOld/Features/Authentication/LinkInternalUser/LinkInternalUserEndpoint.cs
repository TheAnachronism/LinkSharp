using FastEndpoints;
using FluentValidation;

namespace LinkSharp.Features.Authentication.LinkInternalUser;

public class LinkInternalUserRequest
{
    public string UsernameOrEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LinkInternalUserRequestValidator : Validator<LinkInternalUserRequest>
{
    public LinkInternalUserRequestValidator()
    {
        RuleFor(x => x.UsernameOrEmail).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LinkInternalUserEndpoint : Endpoint<LinkInternalUserRequest>
{
    public override void Configure()
    {
        Post("/api/auth/link-internal");
    }

    public override async Task HandleAsync(LinkInternalUserRequest req, CancellationToken ct)
    {
        var result = await new LinkInternalUserCommand(req.UsernameOrEmail, req.Password).ExecuteAsync(ct);
        if (result.IsFailed)
            await SendUnauthorizedAsync();
        else
            await SendOkAsync();
    }
}