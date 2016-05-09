using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Configuration;

namespace ContractOnline
{
    public class Utilities
    {
        static Utilities()
        {
            _client = new SmtpClient(ConfigurationManager.AppSettings["server"],
                                int.Parse(ConfigurationManager.AppSettings["port"]));
            _client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["username"],
                                                                    ConfigurationManager.AppSettings["password"]);
            _client.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["enableSsl"]);
            _client.Timeout = int.Parse(ConfigurationManager.AppSettings["timeout"]);

            _fromAddress = new MailAddress(ConfigurationManager.AppSettings["senderAddress"], ConfigurationManager.AppSettings["senderDisplayName"]);
        }

        public static Image Base64ToImage(string base64String)
        {

            base64String = base64String.Replace("data:image/png;base64,", "");

            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

        public static string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to base 64 string
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        private static SmtpClient _client;
        private static MailAddress _fromAddress;
        private static object _locker = new object();

        public static bool Send(string[] addresses, string[] pdfPaths)
        {
            // Specify the message content.
            MailMessage message = new MailMessage(_fromAddress, new MailAddress(ConfigurationManager.AppSettings["ccAddress"]));
            foreach(var addr in addresses)
            {
                if (!string.IsNullOrEmpty(addr))
                {
                    message.To.Add(addr);
                }
            }
       
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ccAddress1"]))
            {
                message.To.Add(ConfigurationManager.AppSettings["ccAddress1"]);
            }

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ccAddress2"]))
            {
                message.To.Add(ConfigurationManager.AppSettings["ccAddress2"]);
            }
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ccAddress3"]))
            {
                message.To.Add(ConfigurationManager.AppSettings["ccAddress3"]);
            }

            message.Subject = "Your Greenspot Application";
            message.Body = "Your Greenspot Application";
            foreach (string pdf in pdfPaths)
            {
                message.Attachments.Add(new Attachment(pdf));
            }

            try
            {
                _client.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                // Clean up.
                message.Dispose();
            }
        }

        ~Utilities()
        {
            _client.Dispose();
        }
    }
}