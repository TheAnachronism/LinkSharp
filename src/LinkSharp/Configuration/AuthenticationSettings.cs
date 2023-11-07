using System.ComponentModel.DataAnnotations;

namespace LinkSharp.Configuration;

public class AuthenticationSettings
{
    public List<OidcSettings> OidcProviders { get; set; } = null!;
}

public class OidcSettings
{
    [Required] public string ProviderName { get; set; } = null!;
    [Required] public string Authority { get; set; } = null!;
    [Required] public string ClientId { get; set; } = null!;
    [Required] public string ClientSecret { get; set; } = null!;
}