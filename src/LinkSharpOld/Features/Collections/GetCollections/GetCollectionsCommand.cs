using System.Security.Claims;
using FastEndpoints;
using FluentResults;
using LinkSharp.Database;

namespace LinkSharp.Features.Collections.GetCollections;

public class GetCollectionsCommand : ICommand<Result<List<Collection>>>
{
    public ClaimsPrincipal User { get; }

    public GetCollectionsCommand(ClaimsPrincipal user)
    {
        User = user;
    }
}