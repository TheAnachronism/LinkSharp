using FastEndpoints;
using FluentResults;
using LinkSharp.Database;
using LinkSharp.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LinkSharp.Features.Collections.DeleteCollection;

public class DeleteCollectionCommandHandler : ICommandHandler<DeleteCollectionCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public DeleteCollectionCommandHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<Result> ExecuteAsync(DeleteCollectionCommand command, CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(command.User);
        if (user is null)
            return Result.Fail(new UnauthorizedError());

        var collection = await _dbContext.Collections
            .Where(x => x.Id == command.CollectionId)
            .Include(x => x.UserAccesses)
            .ThenInclude(applicationUserCollectionAccess => applicationUserCollectionAccess.ApplicationUser)
            .FirstOrDefaultAsync(cancellationToken: ct);


        if (collection is null || collection.UserAccesses.All(x => x.ApplicationUser != user))
            return Result.Fail(new UnauthorizedError());

        _dbContext.Collections.Remove(collection);
        await _dbContext.SaveChangesAsync(ct);

        return Result.Ok();
    }
}