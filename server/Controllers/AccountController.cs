namespace BffMicrosoftEntraID.Server.Controllers;

// orig src https://github.com/berhir/BlazorWebAssemblyCookieAuth
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    [HttpGet("Login")]
    public ActionResult Login(string? returnUrl, string? claimsChallenge)
    {
        // var claims = "{\"access_token\":{\"acrs\":{\"essential\":true,\"value\":\"c1\"}}}";
        // var claims = "{\"id_token\":{\"acrs\":{\"essential\":true,\"value\":\"c1\"}}}";

        var properties = GetAuthProperties(returnUrl);

        if (claimsChallenge != null)
        {
            string jsonString = claimsChallenge.Replace("\\", "").Trim('"');

            properties.Items["claims"] = jsonString;
        }

        return Challenge(properties);
    }

    // [ValidateAntiForgeryToken] // not needed explicitly due the the Auto global definition.
    [Authorize]
    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        return SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Original src:
    /// https://github.com/dotnet/blazor-samples/blob/main/8.0/BlazorWebOidc/BlazorWebOidc/LoginLogoutEndpointRouteBuilderExtensions.cs
    /// </summary>
    private static AuthenticationProperties GetAuthProperties(string? returnUrl)
    {
        const string pathBase = "/";

        // Prevent open redirects.
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = pathBase;
        }
        else if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
        {
            returnUrl = new Uri(returnUrl, UriKind.Absolute).PathAndQuery;
        }
        else if (returnUrl[0] != '/')
        {
            returnUrl = $"{pathBase}{returnUrl}";
        }

        return new AuthenticationProperties { RedirectUri = returnUrl };
    }
}
