using FastEndpoints;
using FluentResults;

namespace LinkSharp.Features.Authentication.Login;

public class LoginCommand : ICommand<Result>
{
    public string UsernameOrEmail { get; }
    public string Password { get; }

    public LoginCommand(string usernameOrEmail, string password)
    {
        UsernameOrEmail = usernameOrEmail;
        Password = password;
    }
}