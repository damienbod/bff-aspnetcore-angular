namespace BffMicrosoftEntraID.Server;

public class RejectSessionCookieWhenAccountNotInCacheEvents(string[] downstreamScopes) : CookieAuthenticationEvents
{
    public async override Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        try
        {
            var tokenAcquisition = context.HttpContext.RequestServices
                .GetRequiredService<ITokenAcquisition>();

            string token = await tokenAcquisition.GetAccessTokenForUserAsync(scopes: downstreamScopes, 
                user: context.Principal);
        }
        catch (MicrosoftIdentityWebChallengeUserException ex) when (AccountDoesNotExitInTokenCache(ex))
        {
            context.RejectPrincipal();
        }
    }

    private static bool AccountDoesNotExitInTokenCache(MicrosoftIdentityWebChallengeUserException ex)
        => ex.InnerException is MsalUiRequiredException e && e.ErrorCode == "user_null";
}
