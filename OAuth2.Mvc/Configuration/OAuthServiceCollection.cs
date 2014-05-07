﻿using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Text;

namespace OAuth2.Mvc.Configuration
{
    public class OAuthServiceCollection : ProviderCollection
    {
        /// <summary>
        /// Adds a provider to the collection.
        /// </summary>
        /// <param name="provider">The provider to be added.</param>
        /// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="provider"/> is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <see cref="P:System.Configuration.Provider.ProviderBase.Name"/> of <paramref name="provider"/> is null.- or -The length of the <see cref="P:System.Configuration.Provider.ProviderBase.Name"/> of <paramref name="provider"/> is less than 1.</exception>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// </PermissionSet>
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (!(provider is OAuthServiceBase))
                throw new ArgumentException("Provider must implement OAuthServiceBase.");
            base.Add(provider);
        }

        /// <summary>
        /// Gets the <see cref="OAuthServiceBase"/> with the specified name.
        /// </summary>
        /// <value>An instance of <see cref="OAuthProviderBase"/>.</value>
        public new OAuthServiceBase this[string name]
        {
            get
            {
                return (OAuthServiceBase)base[name];
            }
        }
    }
}
