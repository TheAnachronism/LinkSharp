using System.Diagnostics;
using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using FluentResults;
using LinkSharp.Database;
using LinkSharp.Errors;
using LinkSharp.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LinkSharp.Features.Authentication.OpenIdConnectCallback;

public class OidcCallbackHandler : ICommandHandler<OidCallbackCommand, Result>
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public OidcCallbackHandler(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<Result> ExecuteAsync(OidCallbackCommand command, CancellationToken ct)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if(info is null)
            return Result.Fail(new UnauthorizedError());

        var signinResult =
            await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true, true);

        if (signinResult.Succeeded)
            return Result.Ok();

        var email = info.Principal.ClaimValue(ClaimTypes.Email)!;
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return await CreateNewUserAsync(info, email);
        
        if(!(await _userManager.GetLoginsAsync(user)).Any())
            return Result.Fail(new DuplicateEntryError<ApplicationUser>(user, "THis user already exists internally and requires linking!"));

        return await AddNewLoginToUserAsync(info, user);
    }

    private async Task<Result> AddNewLoginToUserAsync(UserLoginInfo info, ApplicationUser user)
    {
        var result = await _userManager.AddLoginAsync(user, info);
        if (!result.Succeeded) result.MapToFluentResultFail();

        await _signInManager.SignInAsync(user, true);
        return Result.Ok();
    }
    
    private async Task<Result> CreateNewUserAsync(ExternalLoginInfo info, string email)
    {
        var username = await GetUniqueUserName(info.Principal);
        var user = new ApplicationUser
        {
            Email = email,
            UserName = username
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded) return result.MapToFluentResultFail();

        result = await _userManager.AddLoginAsync(user, info);

        if (!result.Succeeded) return result.MapToFluentResultFail();

        await _signInManager.SignInAsync(user, true);
        return Result.Ok();
    }
    
    private async Task<string> GetUniqueUserName(ClaimsPrincipal claimsPrincipal)
    {
        var username = GetUsernameFromClaimOptions(claimsPrincipal);

        if (await _userManager.FindByNameAsync(username) is null)
            return username;

        var currentUserCount = await _userManager.Users
            .Where(x => !string.IsNullOrEmpty(x.UserName))
            .Where(x => x.UserName!.ToLower() == username.ToLower())
            .OrderBy(x => x.UserName)
            .CountAsync();

        return $"{username}{currentUserCount + 1}";
    }

    private static string GetUsernameFromClaimOptions(ClaimsPrincipal claimsPrincipal)
    {
        var claimsOptions = new[] { "preferred_username", "nickname", ClaimTypes.Email };

        foreach (var option in claimsOptions)
        {
            var username = claimsPrincipal.ClaimValue(option);
            if (!string.IsNullOrEmpty(username)) return username;
        }

        throw new UnreachableException(
            "At least the mail claim should always be available, so not finding anything is supposed to be impossible.");
    }
}