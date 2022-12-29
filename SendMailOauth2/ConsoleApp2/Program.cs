using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Identity.Client;
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

                var options = new PublicClientApplicationOptions
                {
                    ClientId = "02c3abda-e4a4-4374-9bb3-5fd2aa611533",
                    TenantId = "1b6410fa-9ead-46cf-a6b8-47a23285d047",
                    RedirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient"
                };

                var publicClientApplication = PublicClientApplicationBuilder
                    .CreateWithApplicationOptions(options)
                    .Build();
                var scopes = new string[] {
                "email",
                "offline_access",
                "https://outlook.office.com/IMAP.AccessAsUser.All", // Only needed for IMAP
                //"https://outlook.office.com/SMTP.Send", // Only needed for SMTP
            };

                //var authToken = await publicClientApplication.AcquireTokenInteractive(scopes).ExecuteAsync();
                //var oauth2 = new SaslMechanismOAuth2(authToken.Account.Username, authToken.AccessToken);
                var authToken = await publicClientApplication.AcquireTokenInteractive(scopes).WithLoginHint("nguyentuankhanhcqt@gmail.com").ExecuteAsync();
                await publicClientApplication.AcquireTokenSilent(scopes, authToken.Account).ExecuteAsync();
                SaslMechanism oauth2;



                using (var client = new ImapClient())
                {
                    client.Connect("outlook.office365.com", 993, SecureSocketOptions.SslOnConnect);
                    if (client.AuthenticationMechanisms.Contains("OAUTHBEARER") || client.AuthenticationMechanisms.Contains("XOAUTH2"))
                    {
                        if (client.AuthenticationMechanisms.Contains("OAUTHBEARER"))
                            oauth2 = new SaslMechanismOAuthBearer(authToken.Account.Username, authToken.AccessToken);
                        else
                            oauth2 = new SaslMechanismOAuth2(authToken.Account.Username, authToken.AccessToken);

                        await client.AuthenticateAsync(oauth2);
                    }
                    await client.DisconnectAsync(true);
                }

                //using (var smtpClient = new SmtpClient())
                //{
                //    await smtpClient.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.Auto);
                //    await smtpClient.AuthenticateAsync(oauth2);
                //    await smtpClient.DisconnectAsync(true);

                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                throw ex;
            }

        }
    }
}
