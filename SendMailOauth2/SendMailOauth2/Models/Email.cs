using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Identity.Client;
using MimeKit;
using Newtonsoft.Json.Linq;
using SendMailOauth2.Configs;
using SendMailOauth2.Extension;
using SendMailOAuth2.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SendMailOAuth2.Models
{
    class Email
    {
        static public EmailConfig _EmailConfig;
        public TokenOAuth2 _LocalTokenOAuth2;
        static public string _LocalStoragePath;
        public string Sender { get; set; }
        public string Password { get; set; }
        public string Receiver { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool EnableSsl { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public List<string> Scopes { get; set; }


        public Email()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            Email._LocalStoragePath = Path.Combine(currentDirectory, _EmailConfig.StoragePath);

        }

        /// <summary>
        /// Send mail with SMPTP
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Send()
        {
            try
            {
                if (new FileInfo(Email._LocalStoragePath).Exists)
                {
                    using (var sr = new StreamReader(Email._LocalStoragePath))
                    {
                        string json = sr.ReadToEnd();
                        JObject jConfig = JObject.Parse(json);
                        var jTokenOAuth2 = jConfig.ToObject<JObject>();

                        if (_LocalTokenOAuth2 == null)
                        {
                            _LocalTokenOAuth2 = new TokenOAuth2();
                        }
                        var jGetTokenAccount = jTokenOAuth2["Account"];
                        if (jGetTokenAccount != null)
                        {
                            var jHomeAccountId = jGetTokenAccount["HomeAccountId"] ?? null;
                            _LocalTokenOAuth2.AccessToken = jTokenOAuth2["AccessToken"].ToObject<string>();
                            _LocalTokenOAuth2.ExpiresOn = jTokenOAuth2["ExpiresOn"]?.ToObject<DateTimeOffset>() ?? default(DateTimeOffset);
                            _LocalTokenOAuth2.Account = new GetTokenAccount
                            {
                                Environment = jGetTokenAccount["Environment"].ToObject<string>(),
                                Username = jGetTokenAccount["Username"].ToObject<string>(),
                                HomeAccountId = new Microsoft.Identity.Client.AccountId(
                                    jHomeAccountId["Identifier"].ToObject<string>(),
                                    jHomeAccountId["ObjectId"].ToObject<string>(),
                                    jHomeAccountId["TenantId"].ToObject<string>()
                                )
                            };
                        }
                        sr.Close();
                    }
                }

                Scopes = new List<string>() {
                    //"openid",
                    //"offline_access",
                    "https://outlook.office.com/SMTP.Send"
                };

                #region  Prepare a sample of mail message

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(Sender, Sender));
                message.To.Add(new MailboxAddress("", Receiver));
                message.Subject = Subject;
                message.Body = new TextPart("html") { Text = Content };

                #endregion

                #region Get access Token

                var accessToken = await GetAccessToken();
                if (String.IsNullOrWhiteSpace(accessToken))
                {
                    throw new Exception("Send failed", new Exception($"GetAccessToken fail. accessToken[{accessToken}] is null or empty"));
                }
                #endregion

                #region send mail by SMTP

                //using (var smtpClient = new SmtpClient(new ProtocolLogger("MailLog.log")))
                using (var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync(Host, Port, SecureSocketOptions.StartTls);
                    SaslMechanism oauth2 = new SaslMechanismOAuth2(Sender, accessToken);
                    await smtpClient.AuthenticateAsync(oauth2);
                    Console.WriteLine("smtp send message:", smtpClient.Send(message));
                    await smtpClient.DisconnectAsync(true);
                }

                #endregion

                LogWriter.Write("Send email Successfully.");
                return true;
            }
            catch (Exception ex)
            {
                var message = "Send email fail.";
                LogWriter.WriteException(ex, message);
                return false;
            }
        }

        /// <author>Nguyen Tuan Khanh</author>
        /// <summary>
        /// Get access token by acquire token slient (similar to refresh token)
        /// </summary>
        /// <param name="getAccessTokenByRefreshTokenRequest"></param>
        /// <returns></returns>
        private async Task<AuthenticationResult> GetAccessTokenByRefresh(GetAccessTokenByRefreshRequest getAccessTokenByRefreshTokenRequest)
        {
            try
            {
                var options = new PublicClientApplicationOptions
                {
                    ClientId = getAccessTokenByRefreshTokenRequest.ClientId,
                    TenantId = getAccessTokenByRefreshTokenRequest.TenantId,
                    RedirectUri = getAccessTokenByRefreshTokenRequest.RedirectUri
                };
                var publicClientApplication = PublicClientApplicationBuilder
                    .CreateWithApplicationOptions(options)
                    .Build();
                //var getAccessTokenResponse = await publicClientApplication.AcquireTokenSilent(getAccessTokenByRefreshTokenRequest.Scopes, (IAccount)getAccessTokenByRefreshTokenRequest.Account).ExecuteAsync();

                var accounts = await publicClientApplication.GetAccountsAsync();
                var getAccessTokenResponse = await publicClientApplication.AcquireTokenSilent(getAccessTokenByRefreshTokenRequest.Scopes, accounts.FirstOrDefault()).ExecuteAsync();
                if (getAccessTokenResponse == null)
                {
                    throw new Exception("AcquireTokenSilent failed. getAccessTokenResponse is null.");
                }
                LogWriter.Write($"GetAccessTokenByRefresh success");

                return getAccessTokenResponse;
            }
            catch (Exception ex)
            {
                var message = "GetAccessTokenByRefresh fail";
                LogWriter.WriteException(ex, message);
                return null;
            }
            finally
            {
                //Todo: Write log Data
            }
        }

        /// <author>Nguyen Tuan Khanh</author>
        /// <summary>
        /// Get access token by authorization code grant flow
        /// </summary>
        /// <param name="getAccessTokenByAuthoizationCodeRequest"></param>
        /// <returns></returns>
        private async Task<AuthenticationResult> GetAccessTokenByAuthorizationCode(GetAccessTokenByAuthoizationCodeRequest getAccessTokenByAuthoizationCodeRequest)
        {
            try
            {
                var options = new PublicClientApplicationOptions
                {
                    ClientId = getAccessTokenByAuthoizationCodeRequest.ClientId,
                    TenantId = getAccessTokenByAuthoizationCodeRequest.TenantId,
                    RedirectUri = getAccessTokenByAuthoizationCodeRequest.RedirectUri,
                };
                var publicClientApplication = PublicClientApplicationBuilder
                    .CreateWithApplicationOptions(options)
                    .Build();
                var getAuthorizationCodeResponse = await publicClientApplication
                    .AcquireTokenInteractive(getAccessTokenByAuthoizationCodeRequest.Scopes)
                    .WithLoginHint(getAccessTokenByAuthoizationCodeRequest.Username)
                    .ExecuteAsync();
                if (getAuthorizationCodeResponse == null)
                {
                    throw new Exception("Get token with authorization code grant flow failed. getAuthorizationCodeResponse is null.");
                }
                LogWriter.Write($"Get token with authorization code grant flow success");
                return getAuthorizationCodeResponse;
            }
            catch (Exception ex)
            {
                var message = "Get token with authorization code grant flow";
                LogWriter.WriteException(ex, message);
                return null;
            }
            finally
            {
                //Todo: Write log
            }
        }

        /// <author>Nguyen Tuan Khanh</author>
        /// <summary>
        /// Get access token
        /// </summary>
        /// <returns>string</returns>
        private async Task<string> GetAccessToken()
        {
            try
            {
                if (_LocalTokenOAuth2 != null)
                {
                    #region Khai báo

                    var currentAccessToken = _LocalTokenOAuth2?.AccessToken;
                    var currentAccount = _LocalTokenOAuth2?.Account;
                    var currentExpiresTime = _LocalTokenOAuth2?.ExpiresOn.UtcDateTime;

                    #endregion

                    #region Check if access token is valid

                    if (!String.IsNullOrWhiteSpace(currentAccessToken))
                    {
                        if (DateTime.UtcNow < currentExpiresTime) // if not expire
                        {
                            return currentAccessToken;
                        }
                    }

                    #endregion

                    #region Use current Account data to get access token if having one. Just like refresh token

                    if (currentAccount != null)
                    {
                        //var account = (JObject)currentAccount;
                        var getAccessTokenByRefreshTokenRequest = new GetAccessTokenByRefreshRequest
                        {
                            TenantId = _EmailConfig.TenantId,
                            ClientId = _EmailConfig.ClientId,
                            Scope = String.Join(" ", Scopes),
                            Account = currentAccount,
                            Scopes = this.Scopes,
                        };
                        var getAccessTokeResponse = await GetAccessTokenByRefresh(getAccessTokenByRefreshTokenRequest);
                        if (getAccessTokeResponse == null)
                        {
                            throw new Exception("GetAccessTokenByRefreshToken failed. Cant renew access token");
                        }
                        if (_LocalTokenOAuth2 == null)
                        {
                            _LocalTokenOAuth2 = new TokenOAuth2();
                        }
                        _LocalTokenOAuth2.AccessToken = getAccessTokeResponse.AccessToken;
                        _LocalTokenOAuth2.ExpiresOn = getAccessTokeResponse.ExpiresOn;
                        _LocalTokenOAuth2.Save(_LocalStoragePath);
                        return getAccessTokeResponse.AccessToken;
                    }

                    #endregion
                }

                #region Use Grant code flow to get access token

                var getAccessTokenByAuthorizationCodeRequest = new GetAccessTokenByAuthoizationCodeRequest
                {
                    ClientId = _EmailConfig.ClientId,
                    RedirectUri = _EmailConfig.RedirectUri,
                    TenantId = _EmailConfig.TenantId,
                    Scopes = Scopes,
                    Username = this.Sender
                };
                var getAccessTokenByAuthorizationCodeResponse = await GetAccessTokenByAuthorizationCode(getAccessTokenByAuthorizationCodeRequest);
                if (getAccessTokenByAuthorizationCodeResponse == null)
                {
                    throw new Exception("GetAccessTokenByAuthorizationCode failed. cant get access token");
                }
                if (_LocalTokenOAuth2 == null)
                {
                    _LocalTokenOAuth2 = new TokenOAuth2();
                }
                _LocalTokenOAuth2.AccessToken = getAccessTokenByAuthorizationCodeResponse.AccessToken;
                _LocalTokenOAuth2.ExpiresOn = getAccessTokenByAuthorizationCodeResponse.ExpiresOn;
                _LocalTokenOAuth2.Account = new GetTokenAccount
                {
                    Username = getAccessTokenByAuthorizationCodeResponse.Account.Username,
                    Environment = getAccessTokenByAuthorizationCodeResponse.Account.Environment,
                    HomeAccountId = getAccessTokenByAuthorizationCodeResponse.Account.HomeAccountId
                };
                _LocalTokenOAuth2.Save(_LocalStoragePath);
                LogWriter.Write("Get access token successfully");
                return getAccessTokenByAuthorizationCodeResponse.AccessToken;

                #endregion
            }
            catch (Exception ex)
            {
                var message = "GetAccessToken fail";
                LogWriter.WriteException(ex, message);
                return null;
            }
            finally
            {
                // write log
            }
        }
    }
}
