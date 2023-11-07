using FastEndpoints;
using FluentValidation;
using LinkSharp.Database;
using LinkSharp.Errors;

namespace LinkSharp.Features.Authentication.Register;

public class RegisterRequest
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PasswordConfirm { get; set; } = null!;
}

public class RegisterRequestValidator : Validator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(10);
        RuleFor(x => x.PasswordConfirm).NotEmpty().Equal(x => x.Password);
    }
}

public class RegisterEndpoint: Endpoint<RegisterRequest>
{
    public override void Configure()
    {
        AllowAnonymous();
        Post("/api/auth/register");
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var result = await new RegisterCommand(req.Username, req.Email, req.Password).ExecuteAsync(ct);

        if (result.HasError<DuplicateEntryError<ApplicationUser>>())
            await SendResultAsync(Results.Conflict());
        else if (result.IsFailed)
        {
            foreach (var error in result.Errors) AddError(error.Message);
            await SendErrorsAsync();
        }
        else
            await SendOkAsync();
    }
}