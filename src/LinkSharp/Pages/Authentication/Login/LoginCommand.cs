using FluentResults;
using MediatR;

namespace LinkSharp.Pages.Authentication.Login;

public class LoginCommand : IRequest<Result>
{
    public string UsernameOrEmail { get; }
    public string Password { get; }

    public LoginCommand(string usernameOrEmail, string password)
    {
        UsernameOrEmail = usernameOrEmail;
        Password = password;
    }
}