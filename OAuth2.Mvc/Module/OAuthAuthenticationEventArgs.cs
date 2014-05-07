using System;
using System.Security.Principal;
using System.Web;

namespace OAuth2.Mvc.Module
{
    /// <summary>
    /// Represents the method that handles the OAuthAuthentication_OnAuthenticate event of a 
    /// <see cref="OAuthAuthenticationModule"/>. 
    /// </summary>
    /// <param name="sender">
    /// The object that raised the event. 
    /// </param>
    /// <param name="e">
    /// A <see cref="OAuthAuthenticationEventArgs" /> object that contains the event data. 
    /// </param>
    public delegate void OAuthAuthenticationEventHandler(object sender, OAuthAuthenticationEventArgs e);

    /// <summary>
    /// The event argument passed to the <see cref="E:OAuth2.Mvc.Module.OAuthAuthenticationModule.Authenticate" /> event by a 
    /// <see cref="T:OAuth2.Mvc.Module.OAuthAuthenticationModule" />. Since there is already an identity at this point, 
    /// this is useful mainly for attaching a custom <see cref="T:System.Security.Principal.IPrincipal" /> 
    /// object to the context using the supplied identity.
    /// </summary>
    public sealed class OAuthAuthenticationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthAuthenticationEventArgs"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="identity">The identity.</param>
        /// <param name="context">The context.</param>
        public OAuthAuthenticationEventArgs(IOAuthProvider provider, IIdentity identity, HttpContext context)
        {
            Identity = identity;
            Context = context;
            Provider = provider;
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.HttpContext" /> object for the current HTTP request.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.HttpContext" /> object for the current HTTP request.
        /// </returns>
        public HttpContext Context { get; private set; }

        /// <summary>
        /// Gets an authenticated OAuth identity. 
        /// </summary>
        /// <returns>
        /// An authenticated OAuth identity.
        /// </returns>
        public IIdentity Identity { get; private set; }

        /// <summary>
        /// Gets the OAuth provider used to authenticate.
        /// </summary>
        /// <value>The OAuth provider used to authenticate.</value>
        public IOAuthProvider Provider { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Security.Principal.IPrincipal" /> object to be associated with the request.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Security.Principal.IPrincipal" /> object to be associated with the request.
        /// </returns>
        public IPrincipal User { get; set; }
    }
}
