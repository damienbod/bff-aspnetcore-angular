using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace BffAzureAD.Server.Services;

public class MsGraphService
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly string[] _scopes;

    public MsGraphService(GraphServiceClient graphServiceClient, IConfiguration configuration)
    {
        _graphServiceClient = graphServiceClient;
        var scopes = configuration.GetValue<string>("DownstreamApi:Scopes");
        _scopes = scopes!.Split(' ');
    }

    public async Task<User?> GetGraphApiUser()
    {
        return await _graphServiceClient.Me
            .GetAsync(b => b.Options.WithScopes(_scopes));
    }

    public async Task<string> GetGraphApiProfilePhoto()
    {
        try
        {
            var photo = string.Empty;
            // Get user photo
            using (var photoStream = await _graphServiceClient
                .Me
                .Photo
                .Content
                .GetAsync(b => b.Options.WithScopes(_scopes)))
            {
                byte[] photoByte = ((MemoryStream)photoStream!).ToArray();
                photo = Base64UrlEncoder.Encode(photoByte);
            }

            return photo;
        }
        catch
        {
            return string.Empty;
        }
    }
}

