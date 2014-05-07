using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace OAuth2.Mvc
{
    public static class OAuthExtensions
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string GetToken(this HttpRequest request)
        {
            var wrapper = new HttpRequestWrapper(request);
            return GetToken(wrapper);
        }

        public static string GetToken(this HttpRequestBase request)
        {
            if (request == null)
                return string.Empty;

            // Find Header
            string headerText = request.Headers[OAuthConstants.AuthorzationHeader];
            log.DebugFormat("headerText={0}", headerText);
            if (!string.IsNullOrEmpty(headerText))
            {
                AuthorizationHeader header = new AuthorizationHeader(headerText);

                log.DebugFormat("header.Scheme={0}", header.Scheme);

                //CG: support both Bearer and OAuth Schemes, see http://tools.ietf.org/html/rfc6750#page-5:
                if ("OAuth".Equals(header.Scheme, StringComparison.OrdinalIgnoreCase) ||
                    "Bearer".Equals(header.Scheme, StringComparison.OrdinalIgnoreCase))
                    return header.ParameterText.Trim();
            }

            // Find Clean Param
            var token = request.Params[OAuthConstants.AuthorzationParam];
            log.DebugFormat("token from param={0}", token);
            return !string.IsNullOrEmpty(token)
                ? token.Trim()
                : string.Empty;
        }

        public static string ToSHA1(this string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");

            var hasher = new SHA1Managed();
            var data = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            return BitConverter.ToString(data).Replace("-", String.Empty);
        }
    }
}