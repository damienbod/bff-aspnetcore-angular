using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace BffMicrosoftEntraID.Server;

public class RejectSessionCookieWhenAccountNotInCacheEvents : CookieAuthenticationEvents
{
    private readonly string[] _downstreamScopes;

    public RejectSessionCookieWhenAccountNotInCacheEvents(string[] downstreamScopes)
    {
        _downstreamScopes = downstreamScopes;
    }

    public async override Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        try
        {
            var tokenAcquisition = context.HttpContext.RequestServices
                .GetRequiredService<ITokenAcquisition>();

            string token = await tokenAcquisition.GetAccessTokenForUserAsync(scopes: _downstreamScopes, 
                user: context.Principal);
        }
        catch (MicrosoftIdentityWebChallengeUserException ex) when (AccountDoesNotExitInTokenCache(ex))
        {
            context.RejectPrincipal();
        }
    }

    private static bool AccountDoesNotExitInTokenCache(MicrosoftIdentityWebChallengeUserException ex)
    {
        return ex.InnerException is MsalUiRequiredException 
            && (ex.InnerException as MsalUiRequiredException)!.ErrorCode == "user_null";
    }
}
