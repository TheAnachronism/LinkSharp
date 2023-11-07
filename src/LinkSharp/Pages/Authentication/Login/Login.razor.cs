using MediatR;
using Microsoft.AspNetCore.Components;

namespace LinkSharp.Pages.Authentication.Login;

public partial class Login
{
    private LoginModel _model = new();
    private bool _isUnauthorized;
    [Inject] public IMediator Mediator { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; }

    private async Task LoginAsync()
    {
        var result = await Mediator.Send(new LoginCommand(_model.UsernameOrEmail, _model.Password));
        if (result.IsFailed)
            _isUnauthorized = true;
        else
            NavigationManager.NavigateTo("/");
    }
}