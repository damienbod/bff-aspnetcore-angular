using BffAzureAD.Server.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BffAzureAD.Server.Controllers;

[ValidateAntiForgeryToken]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[AuthorizeForScopes(Scopes = new string[] { "User.ReadBasic.All user.read" })]
[ApiController]
[Route("api/[controller]")]
public class GraphApiDataController : ControllerBase
{
    private readonly MsGraphService _graphApiClientService;

    public GraphApiDataController(MsGraphService graphApiClientService)
    {
        _graphApiClientService = graphApiClientService;
    }

    [HttpGet]
    public async Task<IEnumerable<string>> Get()
    {
        var userData = await _graphApiClientService.GetGraphApiUser();
        if (userData == null)
            return new List<string> { "no user data" };

        return new List<string> { $"DisplayName: {userData.DisplayName}",
            $"GivenName: {userData.GivenName}", $"AboutMe: {userData.AboutMe}" };
    }
}
