using FastEndpoints;
using FluentResults;
using LinkSharp.Database;
using LinkSharp.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LinkSharp.Features.Collections.GetCollections;

public class GetCollectionsCommandHandler : ICommandHandler<GetCollectionsCommand, Result<List<Collection>>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public GetCollectionsCommandHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<Result<List<Collection>>> ExecuteAsync(GetCollectionsCommand command, CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(command.User);
        if (user is null)
            return Result.Fail(new UnauthorizedError());

        var collections = await _dbContext.Collections
            .Where(collection => collection.UserAccesses.Any(access => access.ApplicationUser == user))
            .ToListAsync(cancellationToken: ct);

        return Result.Ok(collections);
    }
}