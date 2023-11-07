using System.Security.Claims;
using FastEndpoints;
using FluentResults;

namespace LinkSharp.Features.Collections.DeleteCollection;

public class DeleteCollectionCommand : ICommand<Result>
{
    public ClaimsPrincipal User { get; }
    public Guid CollectionId { get; }

    public DeleteCollectionCommand(ClaimsPrincipal user, Guid collectionId)
    {
        User = user;
        CollectionId = collectionId;
    }
}