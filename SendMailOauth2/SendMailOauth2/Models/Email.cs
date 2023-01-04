using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using SendMailOAuth2.Config;
using Microsoft.Identity.Client;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using SendMailOauth2.Models;
using System.Web;
using System.Net;
using System.Net.Http;
using SendMailOauth2.Extension;
using System.Net.Http.Headers;
using MailKit.Net.Imap;
using System.Threading.Tasks;

namespace SendMailOAuth2.Models
{
    class Email
    {
        static EmailConfig _EmailConfig;
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool EnableSsl { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public Email()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var staticConfigDirectory = Path.Combine(currentDirectory, "StaticConfig");
            var filePaths = Directory.GetFiles(staticConfigDirectory, "EmailConfig.json", SearchOption.TopDirectoryOnly);
            using (var sr = new StreamReader(filePaths[0]))
            {
                string json = sr.ReadToEnd();
                JObject jConfig = JObject.Parse(json);
                _EmailConfig = jConfig["EmailConfig"].ToObject<EmailConfig>();
            }
        }
        public async Task<bool> Send()
        {
            try
            {
                var clientId = _EmailConfig.ClientId;
                var tenantId = _EmailConfig.TenantId;
                var scopes = new string[] { "https://outlook.office.com/SMTP.Send" };
                var redirectUri = _EmailConfig.RedirectUri;
                var getAccessTokenUri = _EmailConfig.GetTokenUri;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(Sender, Sender));
                //message.From.Add(new MailboxAddress("notify@kiena.vn", "notify@kiena.vn"));
                message.To.Add(new MailboxAddress("", Receiver));
                message.Subject = Subject;
                message.Body = new TextPart("html") { Text = Content };

                #region  Get authorization code

                string authorizationRequest = string.Format(_EmailConfig.AuthorizationUri,
                    clientId,
                    redirectUri,
                    HttpUtility.UrlEncode(String.Join(" ", scopes))
                );
                var httpListener = new HttpListener();
                httpListener.Prefixes.Add(redirectUri);
                httpListener.Start();
                System.Diagnostics.Process.Start(authorizationRequest);     // Opens request in the browser.
                HttpListenerContext context = httpListener.GetContext();
                HttpListenerRequest request = context.Request;
                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                string responseString = string.Format("<html><head></head><body>Please return to the app and close current window.</body></html>");
                var buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                var responseOutput = response.OutputStream;
                Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
                {
                    responseOutput.Close();
                    httpListener.Stop();
                    Console.WriteLine("HTTP server stopped.");
                });
                // Checks for errors.
                if (!String.IsNullOrWhiteSpace(request.QueryString.Get("error")))
                {
                    Console.WriteLine(string.Format("OAuth authorization error: {0}.", request.QueryString.Get("error")));
                    return false;
                }
                if (String.IsNullOrWhiteSpace(request.QueryString.Get("code")))
                {
                    Console.WriteLine("Malformed authorization response. " + request.QueryString);
                    return false;
                }
                // extracts the code
                var authorizartionCode = context.Request.QueryString.Get("code");
                Console.WriteLine("Authorization code: " + authorizartionCode);

                #endregion

                #region Get access Token

                var requestBody = new[]
                {
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("scope", String.Join(" ", scopes)),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", authorizartionCode)
                };

                HttpClient httpClient = new HttpClient();
                var result = await httpClient.PostAsync(getAccessTokenUri, new FormUrlEncodedContent(requestBody));
                // Save the token for further requests.
                var tokenObjString = await result.Content.ReadAsStringAsync();

                var token = tokenObjString.FromJson<Token>();
                if (!String.IsNullOrWhiteSpace(token.Error))
                {
                    Console.WriteLine($"Has Error: {token.Error}.");
                    Console.WriteLine($"Message {token.ErrorDescription}.");
                    Console.WriteLine();
                    throw new Exception(token.ErrorDescription);
                }
                #endregion

                using (var smtpClient = new SmtpClient(new ProtocolLogger(Console.OpenStandardOutput())))
                {
                    await smtpClient.ConnectAsync(Host, Port, SecureSocketOptions.StartTls);
                    SaslMechanism oauth2 = new SaslMechanismOAuth2(Sender, token.AccessToken);
                    await smtpClient.AuthenticateAsync(oauth2);
                    smtpClient.Send(message);
                    await smtpClient.DisconnectAsync(true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "." + ex.InnerException.Message);
                return false;
            }
        }
    }
}
