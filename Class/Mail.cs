using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntegreFDLL.Class.DataTableClass;

namespace EntegrefKrediOnay.Class
{
    public class Mail
    {
        public async Task<string> SendMail(MailRequest request)
        {
            string mailsendsonuc = "";
            try
            {
                string senderEmail = request.Sender.Length > 0 ? request.Sender[0] : "";
                string password = request.Sender.Length > 1 ? request.Sender[1] : "";
                string senderName = request.Sender.Length > 2 ? request.Sender[2] : senderEmail;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));

                foreach (var recipient in request.To)
                    message.To.Add(new MailboxAddress("", recipient));

                message.Subject = request.Subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = request.Body;

                // Ekler
                if (request.AttachmentPaths != null)
                {
                    foreach (var attachmentPath in request.AttachmentPaths)
                    {
                        if (File.Exists(attachmentPath))
                            builder.Attachments.Add(attachmentPath);
                    }
                }

                message.Body = builder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(request.MailHost, request.MailPort, SecureSocketOptions.Auto);
                    client.Authenticate(senderEmail, password);
                    mailsendsonuc = await client.SendAsync(message);
                    client.Disconnect(true);
                };
                return mailsendsonuc;
            }
            catch (Exception ex)
            {
                mailsendsonuc = ex.Message;
                return mailsendsonuc;
            }
        }
    }
}
