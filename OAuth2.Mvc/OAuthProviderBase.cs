using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Security.Principal;
using System.Web;

namespace OAuth2.Mvc
{
    public interface IOAuthProvider
    {
        IIdentity RetrieveIdentity(HttpContext context);
        IPrincipal CreatePrincipal(IIdentity identity);

        void Initialize(string name, NameValueCollection config);
        string Name { get; }
        string Description { get; }
    }

    public abstract class OAuthProviderBase : ProviderBase, IOAuthProvider
    {
        public abstract IIdentity RetrieveIdentity(HttpContext context);

        public abstract IPrincipal CreatePrincipal(IIdentity identity);
    }
}