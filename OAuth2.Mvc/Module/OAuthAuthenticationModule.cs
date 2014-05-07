using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using OAuth2.Mvc.Configuration;

namespace OAuth2.Mvc.Module
{
    /// <summary>
    /// Sets the identity of the user for an ASP.NET application when forms 
    /// authentication is OAuth. This class cannot be inherited.
    /// </summary>
    public sealed class OAuthAuthenticationModule : IHttpModule
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static IOAuthProvider _provider;
        private static bool _checked;
        private static bool _required;

        //private static bool s_initialized = false;

        private static OAuthAuthenticationModule s_singleton = null;
        protected static object m_serviceSync = new Object();   // Used to sync start operations

        public static OAuthAuthenticationModule Instance
        {
            get
            {
                if (s_singleton == null)
                {
                    lock (m_serviceSync)
                    {
                        if (s_singleton == null)
                            s_singleton = new OAuthAuthenticationModule();
                    }
                }
                return s_singleton;
            }
        }

        /// <summary>
        /// Occurs when the application authenticates the current request.
        /// </summary>
        public event OAuthAuthenticationEventHandler Authenticate;

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpApplication"/> that provides access 
        /// to the methods, properties, and events common to all  application 
        /// objects within an ASP.NET application.
        /// </param>
        public void Init(HttpApplication context)
        {
            //if (!s_initialized)
            //{
            //    lock (m_serviceSync)
            //    {
                    log.Debug("Init with context");
                    context.AuthenticateRequest += OnAuthenticateRequest;
                    context.EndRequest += OnEndRequest;
                    //s_initialized = true;
            //    }
            //}
        }
        
        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that 
        /// implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        { }

        private void OnAuthenticateRequest(object sender, EventArgs e)
        {
            log.Debug("OnAuthenticateRequest");
            if (_checked && !_required)
                return;

            log.Debug("Taking application");
            var application = sender as HttpApplication;
            if (application == null)
                return;

            log.Debug("Taking context");
            var context = application.Context;
            if (context == null)
                return;

            log.Debug("Checking _checked");
            if (!_checked)
            {
                log.Debug("_checked=false");
                InitializeProvider();
                InitializeService();
                _required = _provider != null;
                _checked = true;
            }

            log.Debug("Checking _required");
            if (!_required)
                return;

            log.Debug("Getting identity");
            var identity = _provider.RetrieveIdentity(context);
            log.Debug("identity is null: " + (identity==null));
            var eventArgs = new OAuthAuthenticationEventArgs(_provider, identity, context);
            log.Debug("Before OnAuthenticate");
            OnAuthenticate(eventArgs);
            log.Debug("After OnAuthenticate");
        }
        
        private void OnEndRequest(object sender, EventArgs e)
        {
            if (!_required)
                return;
            
            var application = sender as HttpApplication;
            if (application == null)
                return;

            var context = application.Context;
            if (context == null)
                return;

            log.DebugFormat("context.Response.StatusCode={0}", context.Response.StatusCode);
            if (context.Response.StatusCode != 401)
            {
                string redirLocation = context.Response.RedirectLocation;
                log.DebugFormat("redirLocation={0}", redirLocation);
                if ((redirLocation != null) && redirLocation.StartsWith("/Access/Login"))
                    ResponseWithUnauthorizedHeader(context, "Bearer");

                return;
            }

            // add challenge header
            log.Debug("add challenge header");
            ResponseWithUnauthorizedHeader(context, "OAuth");
        }

        private void ResponseWithUnauthorizedHeader(HttpContext context, string initOfHeader)
        {
            if ((context.Request.AcceptTypes != null) && (context.Request.AcceptTypes.Length >= 1))
            {
                string acceptType = context.Request.AcceptTypes[0];
                log.DebugFormat("context.Response.StatusCode={0}", context.Response.StatusCode);
                if (acceptType.Equals("application/json") || acceptType.Equals("text/xml"))
                    context.Response.ContentType = context.Request.AcceptTypes[0];
                else
                {
                    //string url = UnauthorizedPage();
                    //RedirectToErrorPage(context, url);
                    return;
                }
            }
            string errorDescr = "The access token expired";
            try 
            { 
                errorDescr = context.Response.Headers[OAuthConstants.CustomHeader];
                context.Response.Headers.Remove(OAuthConstants.CustomHeader);
            }
            catch { }

            string header = string.Format("{0} realm=\"OAuth\", error=\"invalid_token\", error_description=\"{1}\"", initOfHeader, errorDescr);
            context.Response.AddHeader(OAuthConstants.UnauthorizedHeader, header);
            context.Response.StatusCode = 401;
        }

        private string UnauthorizedHeader(HttpContext context)
        {
            if (context.User == null)
                return "OAuth realm=\"OAuth\"";

            var u = context.User as OAuthPrincipalBase;
            if (u == null)
                return "OAuth realm=\"OAuth\"";

            var i = u.Identity as OAuthIdentityBase;
            if (i == null)
                return "OAuth realm=\"OAuth\"";

            string header = String.Format("OAuth realm=\"{0}\"", i.Realm ?? "OAuth");
            if (!String.IsNullOrEmpty(i.Error))
                header += String.Format(", error=\"{0}\"", i.Error);

            return header;
        }

        private void OnAuthenticate(OAuthAuthenticationEventArgs e)
        {
            log.Debug("In OnAuthenticate");
            if (Authenticate != null)
            {
                log.Debug("Before Authenticate");
                Authenticate(this, e);
                log.Debug("After Authenticate");
                if (e.Context.User == null && e.User != null)
                    e.Context.User = e.User;
            }

            if (e.Context.User == null)
            {
                log.Debug("Before CreatePrincipal");
                e.Context.User = e.Provider.CreatePrincipal(e.Identity);
                log.Debug("After CreatePrincipal");
            }
        }

        private static void InitializeProvider()
        {
            log.Debug("InitializeProvider");
            if (_provider != null)
                return;

            var section = ConfigurationManager.GetSection("oauth") as OAuthSection;

            // if there is no oauth section, then don't use this module
            if (section == null)
            {
                log.Warn("No section found in config");
                return;
            }

            var providers = new OAuthProviderCollection();
            ProvidersHelper.InstantiateProviders(section.Providers, providers, typeof(OAuthProviderBase));

            if ((section.DefaultProvider == null) || (providers.Count < 1))
                ThrowConfigurationError(section, "Default OAuth Provider must be specified.");

            _provider = providers[section.DefaultProvider];
            if (_provider == null)
                ThrowConfigurationError(section, "Default OAuth Provider could not be found.");

            log.Info("Auth Provider init complete");
        }

        private static void InitializeService()
        {
            log.Info("Initializing OAuth service");
            if (OAuthServiceBase.Instance != null)
                return;

            var section = ConfigurationManager.GetSection("oauth") as OAuthSection;

            // if there is no oauth section, then don't use this module
            if (section == null)
            {
                log.Warn("No section found in config");
                return;
            }

            var services = new OAuthServiceCollection();
            ProvidersHelper.InstantiateProviders(section.Services, services, typeof(OAuthServiceBase));

            if ((section.DefaultProvider == null) || (services.Count < 1))
                ThrowConfigurationError(section, "Default OAuth Service must be specified.");

            OAuthServiceBase.Instance = services[section.DefaultService];
            if (OAuthServiceBase.Instance == null)
                log.Warn("Cannot initialize OAuthServiceBase.Instance");
            if (_provider == null)
                ThrowConfigurationError(section, "Default OAuth Service could not be found.");

            log.Info("Auth service init complete");
        }

        private static void ThrowConfigurationError(OAuthSection section, string message)
        {
            log.Error("Configuration error:" + message);
            var elementInformation = section.ElementInformation;
            var propertyInformation = elementInformation.Properties["defaultProvider"];

            if (propertyInformation == null)
                throw new ConfigurationErrorsException(message);

            throw new ConfigurationErrorsException(
                message,
                propertyInformation.Source,
                propertyInformation.LineNumber);
        }

        private static void RedirectToErrorPage(HttpContext context, string url)
        {
            if (string.IsNullOrEmpty(url))
                return;

            if (!context.IsCustomErrorEnabled)
                return;

            context.Response.ClearContent();
            context.Response.Redirect(url, true);
        }
        
        private static string UnauthorizedPage()
        {
            // Get the object related to the <identity> section.
            var errorsSection = WebConfigurationManager.GetSection("system.web/customErrors") as CustomErrorsSection;
            if (errorsSection == null)
                return string.Empty;
            
            if (null == errorsSection.Errors["401"])
                return string.Empty;

            string redirectUrl = errorsSection.Errors["401"].Redirect;
            return redirectUrl;
        }
    }
}
