using FastEndpoints;
using FluentValidation;

namespace LinkSharp.Features.Authentication.Login;

public class LoginRequest
{
    public string UsernameOrEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginRequestValidator : Validator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.UsernameOrEmail).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginEndpoint : Endpoint<LoginRequest>
{
    public override void Configure()
    {
        AllowAnonymous();
        Post("/api/auth/login");
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await new LoginCommand(req.UsernameOrEmail, req.Password).ExecuteAsync(ct);

        if (result.IsFailed)
            await SendUnauthorizedAsync();
        else
            await SendOkAsync();
    }
}