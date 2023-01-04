using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Identity.Client;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var ClientId = "db85b30e-5b5e-443f-b7e1-85bcbb5d5222";
                var TenantId = "6fcf7bf2-2eb6-4b21-b92a-914d27ce33a8";
                var Username = "notify@kiena.vn";
                var Password = "12345aAbB@";
                var ClientSecret = "4z38Q~Oi-IfjCY-~lG1aDyP5Q3amXR2SpzQ9Pc8D";


                //var ClientId = "3ee8c20e-8aeb-4d35-9ffc-43110ed12fcd";
                //var TenantId = "1b6410fa-9ead-46cf-a6b8-47a23285d047";
                //var ClientSecret = "ZzL8Q~PNQYrbm99l6a7kw1Cq2gYyvl1HhGy2Ydhl";
                //var Username = "nguyentuankhanhcqt@gmail.com";
                //var Password = "TranAnhQuan#1701";


                var options = new PublicClientApplicationOptions
                {
                    ClientId = ClientId,
                    TenantId = TenantId,
                    RedirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient"
                };

                //var publicClientApplication = PublicClientApplicationBuilder
                //    .CreateWithApplicationOptions(options)
                //    .Build();

                var scopes = new string[] {
                    //"api://db85b30e-5b5e-443f-b7e1-85bcbb5d5222/.default",
                    //"openid",
                    //"https://outlook.office365.com/.default",
                    "https://outlook.office.com/SMTP.Send"
                };

                //var authToken = await publicClientApplication
                //    .AcquireTokenInteractive(scopes)
                //    .ExecuteAsync();
                //var oauth2 = new SaslMechanismOAuth2(authToken.Account.Username, authToken.AccessToken);
                //var authToken = await publicClientApplication.AcquireTokenInteractive(scopes).WithLoginHint("nguyentuankhanhcqt@gmail.com").ExecuteAsync();
                //await publicClientApplication.AcquireTokenSilent(scopes, authToken.Account).ExecuteAsync();
                SaslMechanism oauth2;



                //var confidentialClientApplication = ConfidentialClientApplicationBuilder
                //    .Create(ClientId)
                //    .WithClientSecret(ClientSecret)
                //    .WithAuthority(AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount)
                //    .Build();
                //var authenticationResult = await confidentialClientApplication.AcquireTokenForClient(scopes).ExecuteAsync();

                //IPublicClientApplication publicClientApp = PublicClientApplicationBuilder.CreateWithApplicationOptions(options)
                //    .WithAuthority(AzureCloudInstance.AzurePublic, TenantId)
                //    .Build();
                //IEnumerable<IAccount> accounts = await publicClientApp.GetAccountsAsync();
                //IAccount account = accounts.FirstOrDefault();
                //AuthenticationResult authenticationResult = null;
                //try
                //{
                //    authenticationResult = await publicClientApp.AcquireTokenSilent(new string[] { "https://outlook.office.com/SMTP.Send" }, account)
                //        .ExecuteAsync();
                //}
                //catch
                //{
                //    try
                //    {
                //        authenticationResult = await publicClientApp.AcquireTokenByUsernamePassword(
                //            new string[] { "https://outlook.office.com/SMTP.Send" },
                //            Username,
                //            Password)
                //            .ExecuteAsync();
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine(ex.Message);
                //        Console.WriteLine("FAILED TO LOGIN");
                //        return;
                //    }
                //}

                //var accessToken = authenticationResult.AccessToken;
                var accessToken = "eyJ0eXAiOiJKV1QiLCJub25jZSI6IjBGRG5YUFpZM2JUX3V5TFROVC1lb2RQYTdRdVE4d2VtWU9rSTlDNzNpdkEiLCJhbGciOiJSUzI1NiIsIng1dCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyIsImtpZCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyJ9.eyJhdWQiOiJodHRwczovL291dGxvb2sub2ZmaWNlLmNvbSIsImlzcyI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzZmY2Y3YmYyLTJlYjYtNGIyMS1iOTJhLTkxNGQyN2NlMzNhOC8iLCJpYXQiOjE2NzI4MjY4NTEsIm5iZiI6MTY3MjgyNjg1MSwiZXhwIjoxNjcyODMxMjcwLCJhY2N0IjowLCJhY3IiOiIxIiwiYWlvIjoiQVRRQXkvOFRBQUFBTDFFbGdGWFZNTWU1cjlqMDZzTXQ3bTlpTEVKYTZLS1hYTXNCT0ZraUV3ckE5S1N3V0QrbGk5Y3BjTDBkWVlIcCIsImFtciI6WyJwd2QiXSwiYXBwX2Rpc3BsYXluYW1lIjoiT0F1dGhUZXN0IiwiYXBwaWQiOiJkYjg1YjMwZS01YjVlLTQ0M2YtYjdlMS04NWJjYmI1ZDUyMjIiLCJhcHBpZGFjciI6IjEiLCJlbmZwb2xpZHMiOltdLCJpcGFkZHIiOiIxNC4yNDEuMjI3LjIzOSIsIm5hbWUiOiJOb3RpZnkiLCJvaWQiOiIxOTY4YzNmNS0xMjNmLTQxYjEtYTA2Mi03NGQ0ZTY4OWVjYzQiLCJwdWlkIjoiMTAwMzIwMDI1NURBNkM5NyIsInJoIjoiMC5BVllBOG52UGI3WXVJVXU1S3BGTko4NHpxQUlBQUFBQUFQRVB6Z0FBQUFBQUFBQldBQVkuIiwic2NwIjoiRVdTLkFjY2Vzc0FzVXNlci5BbGwgSU1BUC5BY2Nlc3NBc1VzZXIuQWxsIFNNVFAuU2VuZCBVc2VyLlJlYWQiLCJzaWQiOiJkNDFiZjM5NC0wMTJkLTRiNzUtOTViNC03ZDE0N2Y1MTY3ZmIiLCJzdWIiOiJIaTI5ZlFILWJSakplenZ3VG5ncmdLT2ZqOTJPNmFYQ3RBS01DS0VRSG80IiwidGlkIjoiNmZjZjdiZjItMmViNi00YjIxLWI5MmEtOTE0ZDI3Y2UzM2E4IiwidW5pcXVlX25hbWUiOiJub3RpZnlAa2llbmEudm4iLCJ1cG4iOiJub3RpZnlAa2llbmEudm4iLCJ1dGkiOiJxY0RiUThhNFVVcW52c1l1OTk4cUFBIiwidmVyIjoiMS4wIiwid2lkcyI6WyJiNzlmYmY0ZC0zZWY5LTQ2ODktODE0My03NmIxOTRlODU1MDkiXX0.adp61pNEcoLdxTG1UTvSfFRu0aPN43kyoR_RqYhVHroc4D-g7SxmseEeNMr9Pt6hpWIQgl48VUzhWEAit0i1KPCyObpMlWXEpls0MxMJWMFhbl3PoQSdH2FxOJ_-5yz6NDTYrHVIjA06cizPWMdvE24rYkiYyGWGgUoC0uxikio1JLEXsD-5_iP78Y09XoamQlIM8DgOXNKVNqN7wXsyhjARRaBhknHR2LRV-8eRoVpQ3jInMTn_9HfxVSuK97EJcpHc_n8C5VoKCN6J5rceSsJ1FXu56VK1rVd7dmW06wNLe89WKaR6A__1nfaXr6WXxgbvQlN3TttxIcWQCbTToQ";
                Console.WriteLine(accessToken);
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

                using (var smtpClient = new SmtpClient(new ProtocolLogger(Console.OpenStandardOutput())))
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(Username, Username));
                    message.To.Add(new MailboxAddress("", "19120540@student.hcmus.edu.vn"));
                    message.Subject = "TEST MAIL";
                    message.Body = new TextPart("html") { Text = "Hello, THIS IS A TEST MAIL" };

                    await smtpClient.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                    oauth2 = new SaslMechanismOAuth2(Username, accessToken);
                    await smtpClient.AuthenticateAsync(oauth2);
                    smtpClient.Send(message);
                    await smtpClient.DisconnectAsync(true);
                }
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
