using FastEndpoints;

namespace LinkSharp.Features.Collections.GetCollections;

public class GetCollectionsEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/collection");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await new GetCollectionsCommand(User).ExecuteAsync(ct);

        if (result.IsFailed)
            await SendUnauthorizedAsync(ct);
        else
            await SendOkAsync(result.Value, ct);
    }
}