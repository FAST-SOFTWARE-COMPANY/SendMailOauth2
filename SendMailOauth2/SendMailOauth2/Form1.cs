using Newtonsoft.Json.Linq;
using SendMailOauth2.Configs;
using SendMailOAuth2;
using SendMailOAuth2.Config;
using SendMailOAuth2.Models;
using System;
using System.IO;
using System.Windows.Forms;

namespace SendMailOauth2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            try
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                var staticConfigDirectory = Path.Combine(currentDirectory, "StaticConfig");
                var filePaths = Directory.GetFiles(staticConfigDirectory, "EmailConfig.json", SearchOption.TopDirectoryOnly);
                using (var sr = new StreamReader(filePaths[0]))
                {
                    string json = sr.ReadToEnd();
                    JObject jConfig = JObject.Parse(json);
                    Email._EmailConfig = jConfig["EmailConfig"].ToObject<EmailConfig>();
                }
                Email._LocalStoragePath = Path.Combine(currentDirectory, Email._EmailConfig.StoragePath);
                using (var sr = new StreamReader(Email._LocalStoragePath))
                {
                    string json = sr.ReadToEnd();
                    JObject jConfig = JObject.Parse(json);
                    Email._LocalTokenOAuth2 = jConfig.ToObject<TokenOAuth2>();
                }
            }
            catch (Exception ex)
            {
                var message = "Get Config failed.";
                LogWriter.WriteException(ex, message);
            }
            InitializeComponent();
        }

        private async void SendMailButton_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                StartSend.Text = DateTime.Now.ToString("dd-M-yyyy HH:mm:ss");
                var email = new Email();
                email.Sender = UsernameInputBox.Text;
                email.Password = PasswordInputBox.Text;
                email.Content = BodyInput.Text;
                email.EnableSsl = EnableSslCheckBox.Checked;
                email.Receiver = RecieverInputBox.Text;
                email.Subject = SubjectInputBox.Text;
                email.Host = HostInputBox.Text;
                email.Port = int.Parse(PortInputBox.Text);
                var isSuccess = await email.Send();
                if (isSuccess)
                {

                    ResultBox.Text = "thành công ...";
                }
                else
                {
                    ResultBox.Text = "failed ....";
                }
                EndSend.Text = DateTime.Now.ToString("dd-M-yyyy HH:mm:ss");
            }
            catch (Exception ex)
            {
                var message = String.Format("SendMailButton_Click failed. Has Exception: {0}. {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty);
                LogWriter.WriteException(ex, message);
            }
        }

        private void SendMailForm_Load(object sender, EventArgs e)
        {
            HostInputBox.Text = Email._EmailConfig.Host;
            PortInputBox.Text = Email._EmailConfig.Port.ToString();
            UsernameInputBox.Text = "notify@kiena.vn";
            PasswordInputBox.Text = "12345aAbB@";
            RecieverInputBox.Text = "19120540@student.hcmus.edu.vn";
            TitleInputBox.Text = "Test";
            SubjectInputBox.Text = "Test";
            BodyInput.Text = "This is a test mail";
        }
    }
}
