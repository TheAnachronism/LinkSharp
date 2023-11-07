using System.Security.Claims;
using FastEndpoints;
using LinkSharp.Database;
using Microsoft.AspNetCore.Identity;

namespace LinkSharp.Features.Authentication.GetUserInfo;

public record UserInfo(bool IsAuthenticated, List<ClaimValue> Claims);

public record ClaimValue(string Type, string Value);

public class GetUserInfoEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        AllowAnonymous();
        Get("/api/auth/GetUserInfo");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (User.Identity?.IsAuthenticated == false)
        {
            await SendOkAsync(new UserInfo(false, new List<ClaimValue>()), ct);
            return;
        }

        var result = await new GetUserInfoCommand(User).ExecuteAsync(ct);
        if (result.IsSuccess)
        {
            await SendOkAsync(new UserInfo(true, new List<ClaimValue>
            {
                new(ClaimTypes.NameIdentifier, result.Value.Id),
                new(ClaimTypes.Name, result.Value.UserName)
            }), ct);
            return;
        }

        await SendOkAsync(ct);
    }
}