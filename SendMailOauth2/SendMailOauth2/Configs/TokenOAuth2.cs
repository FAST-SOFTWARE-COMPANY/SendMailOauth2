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
        public IAccount Account { get; set; }             // for maintain to get access token again

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

        public GetTokenAccount()
        {

        }

        public GetTokenAccount(JObject jGetTokenAccount)
        {
            Username = jGetTokenAccount["Username"]?.ToObject<string>() ?? String.Empty;
            Environment = jGetTokenAccount["Environment"]?.ToObject<string>() ?? String.Empty;
            var identifier = jGetTokenAccount["HomeAccountId"]?["Identifier"]?.ToObject<string>() ?? String.Empty;
            var objectId = jGetTokenAccount["HomeAccountId"]?["ObjectId"]?.ToObject<string>() ?? String.Empty;
            var tenantId = jGetTokenAccount["HomeAccountId"]?["TenantId"]?.ToObject<string>() ?? String.Empty;
            HomeAccountId = new AccountId(identifier, objectId, tenantId);
        }

        public bool IsValid()
        {
            if (String.IsNullOrWhiteSpace(Username) || String.IsNullOrWhiteSpace(Environment))
            {
                return false;
            }
            if (HomeAccountId == null)
            {
                return false;
            }
            return true;
        }
    }
}