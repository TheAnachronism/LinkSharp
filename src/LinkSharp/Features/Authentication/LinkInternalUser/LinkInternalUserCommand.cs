using FastEndpoints;
using FluentResults;

namespace LinkSharp.Features.Authentication.LinkInternalUser;

public class LinkInternalUserCommand : ICommand<Result>
{
    public LinkInternalUserCommand(string usernameOrEmail, string password)
    {
        UsernameOrEmail = usernameOrEmail;
        Password = password;
    }

    public string UsernameOrEmail { get; }
    public string Password { get; }

}