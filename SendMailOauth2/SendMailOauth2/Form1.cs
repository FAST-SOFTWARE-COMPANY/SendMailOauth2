using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;
using SendMailOAuth2.Config;
using SendMailOAuth2.Models;
using SendMailOAuth2;

namespace SendMailOauth2
{
    public partial class Form1 : Form
    {
        static EmailConfig _EmailConfig;

        public Form1()
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
            InitializeComponent();
        }

        private async void SendMailButton_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                StartSend.Text = DateTime.Now.ToString("dd-M-yyyy HH:mm:ss");
                var email = new Email();
                email.Sender = UsernameInputBox.Text;
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
                //LogWriter.Write(message);
            }
        }

        private void SendMailForm_Load(object sender, EventArgs e)
        {
            HostInputBox.Text = _EmailConfig.Host;
            PortInputBox.Text = _EmailConfig.Port.ToString();
        }
    }
}
