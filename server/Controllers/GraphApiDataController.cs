namespace BffMicrosoftEntraID.Server.Controllers;

[ValidateAntiForgeryToken]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[AuthorizeForScopes(Scopes = ["User.ReadBasic.All user.read"])]
[ApiController]
[Route("api/[controller]")]
public class GraphApiDataController(MsGraphService graphApiClientService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<string>> Get()
    {
        var userData = await graphApiClientService.GetGraphApiUser();
        if (userData == null)
            return new List<string> { "no user data" };

        return new List<string> { $"DisplayName: {userData.DisplayName}",
            $"GivenName: {userData.GivenName}", $"AboutMe: {userData.AboutMe}" };
    }
}
