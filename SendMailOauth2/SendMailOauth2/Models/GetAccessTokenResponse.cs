using Microsoft.Identity.Client;
using System.Collections.Generic;

namespace SendMailOAuth2.Models
{
    public class GetAccessTokenRequest
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string Scope { get; set; }
        public string RedirectUri { get; set; }
        public List<string> Scopes { get; set; }
        public string Username { get; set; }
    }
    public class GetAccessTokenByRefreshRequest : GetAccessTokenRequest
    {
        public IAccount Account { get; set; }
    }
    public class GetAccessTokenByAuthoizationCodeRequest : GetAccessTokenRequest
    {

    }
}