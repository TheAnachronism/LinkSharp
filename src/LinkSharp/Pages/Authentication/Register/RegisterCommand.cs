using FluentResults;
using MediatR;

namespace LinkSharp.Pages.Authentication.Register;

public class RegisterCommand : IRequest<Result>
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