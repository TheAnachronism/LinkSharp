using System.Security.Claims;
using FastEndpoints;
using FluentResults;

namespace LinkSharp.Features.Collections.CreateCollection;

public class CreateCollectionCommand : ICommand<Result>
{
    public string Name { get; }
    public ClaimsPrincipal Owner { get; }

    public CreateCollectionCommand(string name, ClaimsPrincipal owner)
    {
        Name = name;
        Owner = owner;
    }
}