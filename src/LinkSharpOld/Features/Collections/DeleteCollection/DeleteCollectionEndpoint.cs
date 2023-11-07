using FastEndpoints;
using FluentValidation;

namespace LinkSharp.Features.Collections.DeleteCollection;

public class DeleteCollectionRequest
{
    public Guid CollectionId { get; set; }
}

public class DeleteCollectionRequestValidator : Validator<DeleteCollectionRequest>
{
    public DeleteCollectionRequestValidator()
    {
        RuleFor(x => x.CollectionId).NotEmpty();
    }
}

public class DeleteCollectionEndpoint : Endpoint<DeleteCollectionRequest>
{
    public override void Configure()
    {
        Delete("/api/collection");
    }

    public override async Task HandleAsync(DeleteCollectionRequest req, CancellationToken ct)
    {
        var result = await new DeleteCollectionCommand(User, req.CollectionId).ExecuteAsync(ct: ct);

        if (result.IsFailed)
            await SendUnauthorizedAsync();
        else
            await SendOkAsync();
    }
}