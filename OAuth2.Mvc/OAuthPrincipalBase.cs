using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

namespace OAuth2.Mvc
{
    /// <summary>
    /// Represents an OAuth authenticated principal.
    /// </summary>
    [DebuggerDisplay("Identity: {Identity}")]
    public abstract class OAuthPrincipalBase : IPrincipal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthPrincipalBase"/> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        protected OAuthPrincipalBase(IOAuthProvider provider, IIdentity identity)
        {
            Provider = provider;
            Identity = identity;
        }

        protected IOAuthProvider Provider { get; private set; }

        /// <summary>
        /// Gets the identity of the current principal.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Security.Principal.IIdentity"/> object associated with the current principal.</returns>
        public virtual IIdentity Identity { get; private set; }

        private string[] _roles;

        /// <summary>
        /// Gets or sets an array of role names to which the user represented by the identity.
        /// </summary>
        /// <value>An array of role names.</value>
        public virtual string[] Roles
        {
            get
            {
                Initialize();
                return _roles;
            }
            protected set
            {
                _roles = value;
            }
        }

        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <param name="role">The name of the role for which to check membership.</param>
        /// <returns>
        /// true if the current principal is a member of the specified role; otherwise, false.
        /// </returns>
        public bool IsInRole(string role)
        {
            if (role == null || Roles == null)
                return false;

            return Roles.Any(r =>
                string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
        }

        private bool _initialized;

        protected virtual void Initialize()
        {
            if (_initialized)
                return;

            Load();
            _initialized = true;
        }

        protected abstract void Load();
    }
}