using FastEndpoints;
using FluentResults;

namespace LinkSharp.Features.Authentication.Register;

public class RegisterCommand : ICommand<Result>
{
    public string Username { get; }
    public string Email { get; }
    public string Password { get; }

    public RegisterCommand(string username, string email, string password)
    {
        Username = username;
        Email = email;
        Password = password;
    }
}