using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMailOAuth2.Config
{
    class EmailConfig
    {
        public string Email { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSSl { get; set; }
        public bool IsBodyHtml { get; set; }
        public string RedirectUri { get; set; }
        public string AuthorizationUri { get; set; }
        public string GetTokenUri { get; set; }
        public string StoragePath { get; set; }     // nơi lưu trữ access token và refresh token hiện
    }
}
