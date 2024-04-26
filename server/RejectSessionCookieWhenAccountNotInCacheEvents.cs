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

            await tokenAcquisition.GetAccessTokenForUserAsync(scopes: _downstreamScopes, 
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
