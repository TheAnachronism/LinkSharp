using FastEndpoints;
using FluentValidation;

namespace LinkSharp.Features.Collections.CreateCollection;

public class CreateCollectionRequest
{
    public string Name { get; set; } = null!;
}

public class CreateCollectionRequestValidator : Validator<CreateCollectionRequest>
{
    public CreateCollectionRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

public class CreateCollectionEndpoint : Endpoint<CreateCollectionRequest>
{
    public override void Configure()
    {
        Post("/api/collection");
    }

    public override async Task HandleAsync(CreateCollectionRequest req, CancellationToken ct)
    {
    }
}