using LinkSharp.Database;
using LinkSharp.Errors;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace LinkSharp.Pages.Authentication.Register;

public partial class Register
{
    private RegisterModel _model = new();
    private List<string> _errors = new();
    [Inject] public IMediator Mediator { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;

    private async Task RegisterAsync()
    {
        var result = await Mediator.Send(new RegisterCommand(_model.Username, _model.Email, _model.Password));

        if (result.HasError<DuplicateEntryError<ApplicationUser>>())
        {
            _errors.Clear();
            _errors.Add("User with that username already exists!");
            return;
        }
        
        if (result.IsFailed)
        {
            _errors.Clear();
            _errors.AddRange(result.Errors.Select(x => x.Message));
            return;
        }
        
        NavigationManager.NavigateTo("/");
    }
}