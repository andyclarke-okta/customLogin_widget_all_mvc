using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace OktaJS_SDK.Services
{
    public class EmailMgmt
    {

        private SmtpClient smtpClient;
        private ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EmailMgmt(string host, int port, bool enableSsl, string fromAddress, string fromPassword)
        {
            smtpClient = new SmtpClient(host, port);
            smtpClient.EnableSsl = enableSsl;
            smtpClient.Credentials = new NetworkCredential(fromAddress, fromPassword);
        }

        public void SendEmail(string fromAddress, string fromName, string toAddress, string subject, string body, bool isHtml)
        {
            try
            {
                MailMessage message = new MailMessage();

                message.From = new MailAddress(fromAddress, fromName);
                message.To.Add(toAddress);
                message.Subject = subject;
                message.IsBodyHtml = isHtml;
                message.Body = body;

                logger.Debug("SendEmail " + toAddress + " message " + message.Body.ToString());
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                string errorMessage = "Mail send failed to " + toAddress + " error; " + ex.Message;
                logger.Error(errorMessage);
                throw new Exception(errorMessage);
            }
        }




    }
}