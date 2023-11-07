using System.ComponentModel.DataAnnotations;

namespace LinkSharp.Pages.Authentication.Login;

public class LoginModel
{
    [Required] public string UsernameOrEmail { get; set; } = null!;

    [Required] public string Password { get; set; } = null!;
}