using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Identity.Client;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            SaslMechanism oauth2;
            try
            {
                //var ClientId = "db85b30e-5b5e-443f-b7e1-85bcbb5d5222";
                //var TenantId = "6fcf7bf2-2eb6-4b21-b92a-914d27ce33a8";
                //var Username = "notify@kiena.vn";
                //var Password = "12345aAbB@";
                //var ClientSecret = "LqG8Q~d70XG7558uxbPansV3WvTdW-AAmN_qxc40";


                var ClientId = "3ee8c20e-8aeb-4d35-9ffc-43110ed12fcd";
                var TenantId = "1b6410fa-9ead-46cf-a6b8-47a23285d047";
                var ClientSecret = "ZzL8Q~PNQYrbm99l6a7kw1Cq2gYyvl1HhGy2Ydhl";
                var Username = "nguyentuankhanhcqt@gmail.com";
                var Password = "TranAnhQuan#1701";


                var options = new PublicClientApplicationOptions
                {
                    ClientId = ClientId,
                    TenantId = TenantId,
                    RedirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient"
                };


                //"api://db85b30e-5b5e-443f-b7e1-85bcbb5d5222/.default",
                //"https://outlook.office.com/.default",
                //"openid",
                //"offline_access",
                //"https://outlook.office.com/SMTP.Send",
                //"https://outlook.office.com/IMAP.AccessAsUser.All",
                var scopes = new string[] {
                    "https://graph.microsoft.com/.default"                  // scope to use graph api
                };

                //var publicClientApplication = PublicClientApplicationBuilder
                //    .CreateWithApplicationOptions(options)
                //    .Build();
                //var authenticationResult = await publicClientApplication
                //    .AcquireTokenInteractive(scopes)
                //    .WithLoginHint(Username)
                //    .ExecuteAsync();
                //await publicClientApplication.AcquireTokenSilent(scopes, authenticationResult.Account).ExecuteAsync();

                #region Cient Credentials Flow
                //NOte: Khong dung client credentials flow cho smtp
                var confidentialClientApplication = ConfidentialClientApplicationBuilder
                    .Create(ClientId)
                    .WithClientSecret(ClientSecret)
                    .Build();
                var authenticationResult = await confidentialClientApplication.AcquireTokenForClient(scopes).ExecuteAsync();

                #endregion

                #region ROPC

                //var GetAccessTokenUri = "https://login.microsoftonline.com/organizations/oauth2/v2.0/token";
                //var httpClient = new HttpClient();
                //var requestBody = new[]
                //{
                //    new KeyValuePair<string, string>("client_id", ClientId),
                //    new KeyValuePair<string, string>("scope", String.Join(" ", scopes)),
                //    new KeyValuePair<string, string>("client_secret", ClientSecret),
                //    new KeyValuePair<string, string>("username", Username),
                //    new KeyValuePair<string, string>("password", Password),
                //    new KeyValuePair<string, string>("grant_type", "password"),
                //};
                //var response = await httpClient.PostAsync(GetAccessTokenUri, new FormUrlEncodedContent(requestBody));
                //var reponseString = await response.Content.ReadAsStringAsync();
                //var authenticationResult = reponseString.FromJson<Token>();

                #endregion

                var accessToken = authenticationResult.AccessToken;
                #region IMAP send mail("FAILED")
                //using (var client = new ImapClient(new ProtocolLogger(Console.OpenStandardOutput())))
                //{
                //    client.Connect("outlook.office365.com", 993, SecureSocketOptions.SslOnConnect);
                //    oauth2 = new SaslMechanismOAuth2(Username, accessToken);
                //    client.Authenticate(oauth2);
                //    var inbox = client.Inbox;
                //    inbox.Open(MailKit.FolderAccess.ReadOnly);
                //    Console.WriteLine("Total messages: {0}", inbox.Count);
                //    Console.WriteLine("Recent messages: {0}", inbox.Recent);
                //    client.Disconnect(true);
                //}
                #endregion

                #region SMTP send mail("FAILED")

                //using (var smtpClient = new SmtpClient(new ProtocolLogger(Console.OpenStandardOutput())))
                //{
                //    var message = new MimeMessage();
                //    message.From.Add(new MailboxAddress(Username, Username));
                //    message.To.Add(new MailboxAddress("", "19120540@student.hcmus.edu.vn"));
                //    message.Subject = "TEST MAIL";
                //    message.Body = new TextPart("html") { Text = "Hello, THIS IS A TEST MAIL" };

                //    await smtpClient.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                //    if (smtpClient.AuthenticationMechanisms.Contains("OAUTHBEARER") || smtpClient.AuthenticationMechanisms.Contains("XOAUTH2"))
                //    {
                //        // Note: We use authToken.Account.Username here instead of ExchangeAccount because the user *may* have chosen a
                //        // different Microsoft Exchange account when presented with the browser window during the authentication process.
                //        if (smtpClient.AuthenticationMechanisms.Contains("OAUTHBEARER"))
                //            //oauth2 = new SaslMechanismOAuthBearer(authenticationResult.Account.Username, authenticationResult.AccessToken);
                //            oauth2 = new SaslMechanismOAuthBearer(Username, authenticationResult.AccessToken);
                //        else
                //            //oauth2 = new SaslMechanismOAuth2(authenticationResult.Account.Username, authenticationResult.AccessToken);
                //            oauth2 = new SaslMechanismOAuth2(Username, authenticationResult.AccessToken);
                //        await smtpClient.AuthenticateAsync(oauth2);
                //        smtpClient.Send(message);
                //    }
                //    await smtpClient.DisconnectAsync(true);
                //}

                #endregion

                #region Call Microsoft Graph Api
                var graphUrl = "https://graph.microsoft.com/v1.0/";
                var httpClient = new HttpClient();


                #endregion

            }
            catch (AuthenticationException authenEx)
            {
                Console.WriteLine();
                Console.WriteLine($"Exception: {authenEx.Message}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException ?? ex);
            }
            Console.ReadKey();
        }
    }
}
