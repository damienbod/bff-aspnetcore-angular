using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace BffAzureAD.Server.Services
{
    public class MsGraphService
    {
        private readonly GraphServiceClient _graphServiceClient;

        public MsGraphService(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        public async Task<User?> GetGraphApiUser()
        {
            return await _graphServiceClient.Me
                .GetAsync(b => b.Options.WithScopes("User.ReadBasic.All", "user.read"));
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
                    .GetAsync(b => b.Options.WithScopes("User.ReadBasic.All", "user.read")))
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
}

