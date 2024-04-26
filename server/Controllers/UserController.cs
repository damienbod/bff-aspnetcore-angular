namespace BlazorBffOpenIDConnect.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetCurrentUser() => Ok(CreateUserInfo(User));

    private static UserInfo CreateUserInfo(ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal == null || claimsPrincipal.Identity == null 
            || !claimsPrincipal.Identity.IsAuthenticated)
        {
            return UserInfo.Anonymous;
        }

        var userInfo = new UserInfo
        {
            IsAuthenticated = true
        };

        if (claimsPrincipal.Identity is ClaimsIdentity claimsIdentity)
        {
            userInfo.NameClaimType = claimsIdentity.NameClaimType;
            userInfo.RoleClaimType = claimsIdentity.RoleClaimType;
        }
        else
        {
            userInfo.NameClaimType = ClaimTypes.Name;
            userInfo.RoleClaimType = ClaimTypes.Role;
        }

        if (claimsPrincipal.Claims?.Any() ?? false)
        {
            // Add just the name claim
            var claims = claimsPrincipal.FindAll(userInfo.NameClaimType)
                                        .Select(u => new ClaimValue(userInfo.NameClaimType, u.Value))
                                        .ToList();

            // Uncomment this code if you want to send additional claims to the client.
            //var claims = claimsPrincipal.Claims.Select(u => new ClaimValue(u.Type, u.Value))
            //                                      .ToList();

            userInfo.Claims = claims;
        }

        return userInfo;
    }
}
