using System.Collections.Specialized;
using System.Configuration.Provider;

namespace OAuth2.Mvc
{
    public interface IOAuthService 
    {
        OAuthResponse RequestToken();
        OAuthResponse AccessToken(string requestToken, string grantType, string userName, string password, bool persistent);
        OAuthResponse RefreshToken(string refreshToken);
        bool UnauthorizeToken(string token);

        void Initialize(string name, NameValueCollection config);
        string Name { get; }
        string Description { get; }
    }

    public abstract class OAuthServiceBase : ProviderBase, IOAuthService
    {
        public static IOAuthService Instance { get; set; }

        public abstract OAuthResponse RequestToken();

        public abstract OAuthResponse AccessToken(string requestToken, string grantType, string userName, string password, bool persistent);

        public abstract OAuthResponse RefreshToken(string refreshToken);

        public abstract bool UnauthorizeToken(string token);
    }
}