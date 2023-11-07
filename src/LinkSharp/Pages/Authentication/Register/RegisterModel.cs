using System.ComponentModel.DataAnnotations;

namespace LinkSharp.Pages.Authentication.Register;

public class RegisterModel
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string PasswordConfirm { get; set; }
}