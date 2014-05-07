using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace OAuth2.Mvc
{
    [DataContract(Name = "oauthResponse")]
    [JsonObject(MemberSerialization.OptIn)]
    public class OAuthResponse
    {
        [DataMember(Name = "request_token")]
        [JsonProperty("request_token", NullValueHandling = NullValueHandling.Ignore)]
        public string RequestToken 
        {
            get { return request_token; }
            set { request_token = value; }
        }

        public string request_token { get; set; }

        [DataMember(Name = "access_token")]
        [JsonProperty("access_token", NullValueHandling = NullValueHandling.Ignore)]
        public string AccessToken 
        {
            get { return access_token; }
            set { access_token = value; }
        }

        public string access_token { get; set; }

        [DefaultValue(0)]
        [DataMember(Name = "expires_in")]
        [JsonProperty("expires_in", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Expires
        {
            get { return expires_in; }
            set { expires_in = value; }
        }

        public int expires_in { get; set; }

        [DataMember(Name = "refresh_token")]
        [JsonProperty("refresh_token", NullValueHandling = NullValueHandling.Ignore)]
        public string RefreshToken
        {
            get { return refresh_token; }
            set { refresh_token = value; }
        }

        public string refresh_token { get; set; }

        [DataMember(Name = "scope")]
        [JsonProperty("scope", NullValueHandling = NullValueHandling.Ignore)]
        public string Scope
        {
            get { return scope; }
            set { scope = value; }
        }

        public string scope { get; set; }

        [DataMember(Name = "error")]
        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public string Error
        {
            get { return error; }
            set { error = value; }
        }

        public string error { get; set; }

        [DataMember(Name = "success")]
        [JsonProperty("success")]
        public bool Success
        {
            get { return success; }
            set { success = value; }
        }
        public bool success { get; set; }

        [DefaultValue(false)]
        [DataMember(Name = "require_ssl")]
        [JsonProperty("require_ssl", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool RequireSsl
        {
            get { return require_ssl; }
            set { require_ssl = value; }
        }

        public string token_type { get { return "bearer"; } }

        public bool require_ssl { get; set; }

        public void Clear()
        {
            AccessToken = null;
            Expires = 0;
            RefreshToken = null;
            Scope = null;
            Error = null;
            RequireSsl = false;
            Success = false;
        }

        public void SetExpires(DateTime expireDate)
        {
            Expires = (int)expireDate.Subtract(DateTime.Now).TotalSeconds;
        }

        public override string ToString()
        {
            return string.Format("Success:{0}, Error={1}, RequestToken={2}, AccessToken={3}, Expires={4}, RequireSsl={5}",
                success, Null(error), Null(RequestToken), Null(AccessToken), Expires, RequireSsl);
        }

        private static string Null(string input)
        {
            return (input == null ? "null" : input);
        }
    }
}