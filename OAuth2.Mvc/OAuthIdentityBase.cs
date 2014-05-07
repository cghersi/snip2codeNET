using System.Diagnostics;
using System.Security.Principal;

namespace OAuth2.Mvc
{
    /// <summary>
    /// Represents a user identity authenticated using OAuth authentication.
    /// </summary>
    [DebuggerDisplay("Token: {Token}")]
    public abstract class OAuthIdentityBase : IIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthIdentityBase"/> class.
        /// </summary>
        protected OAuthIdentityBase(IOAuthProvider provider)
        {
            Provider = provider;
        }

        protected IOAuthProvider Provider { get; private set; }

        public string Realm { get; protected set; }

        public string Error { get; protected set; }

        /// <summary>
        /// Gets the type of authentication used.
        /// </summary>
        /// <value></value>
        /// <returns>The type of authentication used to identify the user.</returns>
        public virtual string AuthenticationType
        {
            get { return "OAuth"; }
        }

        private string _name;

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the user on whose behalf the code is running.</returns>
        public virtual string Name
        {
            get
            {
                Initialize();
                return _name;
            }
            protected set
            {
                _name = value;
            }
        }

        private bool _isAuthenticated;

        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated.
        /// </summary>
        /// <value></value>
        /// <returns><c>true</c> if the user was authenticated; otherwise, <c>false</c>.</returns>
        public virtual bool IsAuthenticated
        {
            get
            {
                Initialize();
                return _isAuthenticated;
            }
            protected set
            {
                _isAuthenticated = value;
            }
        }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>The token.</value>
        public string Token { get; set; }

        private bool _initialized;

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected virtual void Initialize()
        {
            if (_initialized)
                return;

            Load();
            _initialized = true;
        }

        /// <summary>
        /// Triggers the loading and authentication of the identity. 
        /// The method is responsible for setting <see cref="IsAuthenticated"/>.
        /// </summary>
        protected abstract void Load();
    }
}
