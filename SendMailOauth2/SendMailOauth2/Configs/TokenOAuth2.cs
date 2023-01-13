using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace SendMailOauth2.Configs
{
    class TokenOAuth2
    {
        public string AccessToken { get; set; }
        public DateTimeOffset ExpiresOn { get; set; }
        public GetTokenAccount Account { get; set; }             // for maintain to get access token again

        public TokenOAuth2()
        {

        }

        public TokenOAuth2(string accessToken, DateTimeOffset expires, GetTokenAccount account)
        {
            AccessToken = accessToken;
            ExpiresOn = expires;
            Account = account;
        }

        public bool Save(string Path)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                System.IO.File.WriteAllText(Path, jsonString);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException ?? ex);
            }
        }
    }

    public class GetTokenAccount : IAccount
    {
        public string Username { get; set; }
        public string Environment { get; set; }
        public AccountId HomeAccountId { get; set; }
    }
}