using FastEndpoints;
using FluentResults;
using LinkSharp.Database;
using LinkSharp.Errors;
using Microsoft.AspNetCore.Identity;

namespace LinkSharp.Features.Collections.CreateCollection;

public class CreateCollectionCommandHandler : ICommandHandler<CreateCollectionCommand, Result>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateCollectionCommandHandler(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result> ExecuteAsync(CreateCollectionCommand command, CancellationToken ct)
    {
        var owner = await _userManager.GetUserAsync(command.Owner);
        if (owner is null)
            return Result.Fail(new UnauthorizedError());

        var collection = new Collection { Name = command.Name };
        var access = new ApplicationUserCollectionAccess
            { ApplicationUser = owner, Collection = collection, CollectionAccessLevel = CollectionAccessLevel.Owner };
        collection.UserAccesses.Add(access);

        await _dbContext.Collections.AddAsync(collection, ct);
        owner.Collections.Add(access);

        await _userManager.UpdateAsync(owner);
        await _dbContext.SaveChangesAsync(ct);

        return Result.Ok();
    }
}