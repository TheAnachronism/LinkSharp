using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace LinkSharp.Extensions;

public static class IdentityResultExtensions
{
    public static Result MapToFluentResultFail(this IdentityResult result) =>
        Result.Fail(result.Errors.Select(x => new Error(x.Description)));
}