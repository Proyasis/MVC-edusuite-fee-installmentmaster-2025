using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using CITS.EduSuite.Business.Models.ViewModels;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Threading;
using CITS.EduSuite.Business.Services;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.UI
{
    public class EmailHelper
    {

        static string EmailFrom = ConfigurationManager.AppSettings["EmailFrom"].ToString();
        static string EmailTo = ConfigurationManager.AppSettings["EmailTo"].ToString();
        static string EmalSSLEnable = ConfigurationManager.AppSettings["EmalSSLEnable"].ToString();
        static string EmailPassword = ConfigurationManager.AppSettings["EmailPassword"].ToString();
        static string EmailPort = ConfigurationManager.AppSettings["EmailPort"].ToString();
        static string EmailSmtpClient = ConfigurationManager.AppSettings["EmailSmtpClient"].ToString();
        public void SendMail(EmailViewModel model)
        {

            WebMail.SmtpServer = "smtp.gmail.com";
            //gmail port to send emails  
            WebMail.SmtpPort = 587;
            WebMail.SmtpUseDefaultCredentials = true;
            //sending emails with secure protocol  
            WebMail.EnableSsl = true;
            //EmailId used to send emails from application  
            WebMail.UserName = "edusuitemail@gmail.com";
            var encriptString = CryptographicHelper.Encryptor("citsedusuite", DbConstants.EncryptionKey);
            WebMail.Password = CryptographicHelper.DecryptFromBase64String(encriptString, DbConstants.EncryptionKey); ;

            //Sender email address.  
            WebMail.From = "edusuitemail@gmail.com";

            //Send email  
            WebMail.Send(to: model.EmailTo, subject: model.EmailSubject, body: model.EmailBody, isBodyHtml: true);
        }

        public static void SendMailWithAttachment(EmailViewModel model)
        {
            

            WebMail.SmtpServer = "smtp.gmail.com";
            //gmail port to send emails  
            WebMail.SmtpPort = 587;
            WebMail.SmtpUseDefaultCredentials = true;
            //sending emails with secure protocol  
            WebMail.EnableSsl = true;
            //EmailId used to send emails from application  
            WebMail.UserName = "edusuitemail@gmail.com";
            var encriptString = CryptographicHelper.Encryptor("citsedusuite", DbConstants.EncryptionKey);
            WebMail.Password = CryptographicHelper.DecryptFromBase64String(encriptString, DbConstants.EncryptionKey); ;

            //Sender email address.  
            WebMail.From = "edusuitemail@gmail.com";

            //Send email  
            WebMail.Send(to: model.EmailTo, subject: model.EmailSubject, body: model.EmailBody, filesToAttach: model.EmailAttachment, isBodyHtml: true);
        }


        public static bool SendEmail(EmailViewModel model)
        {
            try
            {
                MailMessage mailMsg = new MailMessage();
                mailMsg.Subject = model.EmailSubject;
                mailMsg.Body = model.EmailBody;
                mailMsg.To.Add(new MailAddress(model.EmailTo));
                if (model.EmailCC != null)
                    mailMsg.Bcc.Add(new MailAddress(model.EmailCC));
                mailMsg.From = new MailAddress(EmailFrom);

                foreach (string Attachment in model.EmailAttachment)
                {
                    mailMsg.Attachments.Add(new Attachment(Attachment));
                }

                mailMsg.IsBodyHtml = true;
                SmtpClient smtpClient = new SmtpClient(EmailSmtpClient);
                NetworkCredential networkCredential = new NetworkCredential();
                networkCredential.UserName = mailMsg.From.ToString();
                var encriptString = CryptographicHelper.Encryptor(EmailPassword, DbConstants.EncryptionKey);
                networkCredential.Password = CryptographicHelper.DecryptFromBase64String(encriptString, DbConstants.EncryptionKey);
                smtpClient.Credentials = networkCredential;
                smtpClient.Port = Convert.ToInt32(EmailPort);
                //If you are using gmail account then
                smtpClient.EnableSsl = Convert.ToBoolean(EmalSSLEnable);

                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtpClient.Send(mailMsg);
                ActivityLog.CreateActivityLog("EMAIL","SEND", DbConstants.LogType.Info, DbConstants.User.UserKey,EduSuiteUIResources.Success);
                return true;
            }
            catch (SmtpException ex)
            {
                
             
                ActivityLog.CreateActivityLog("EMAIL", "SEND", DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return false;
            }
        }

        public static bool SendEmailMultiple(EmailViewModel model)
        {
            try
            {
                MailMessage mailMsg = new MailMessage();
                mailMsg.Subject = model.EmailSubject;
                mailMsg.Body = model.EmailBody;

                foreach (string multiEmailId in model.EmailTolist)
                {
                    mailMsg.To.Add(new MailAddress(multiEmailId));
                }
                //mailMsg.To.Add(new MailAddress(model.EmailTo));
                if (model.EmailCC != null)
                    mailMsg.Bcc.Add(new MailAddress(model.EmailCC));
                mailMsg.From = new MailAddress(EmailFrom);

                foreach (string Attachment in model.EmailAttachment)
                {
                    mailMsg.Attachments.Add(new Attachment(Attachment));
                }

                mailMsg.IsBodyHtml = true;
                SmtpClient smtpClient = new SmtpClient(EmailSmtpClient);
                NetworkCredential networkCredential = new NetworkCredential();
                networkCredential.UserName = mailMsg.From.ToString();
                var encriptString = CryptographicHelper.Encryptor(EmailPassword, DbConstants.EncryptionKey);
                networkCredential.Password = CryptographicHelper.DecryptFromBase64String(encriptString, DbConstants.EncryptionKey);
                smtpClient.Credentials = networkCredential;
                smtpClient.Port = Convert.ToInt32(EmailPort);
                //If you are using gmail account then
                smtpClient.EnableSsl = Convert.ToBoolean(EmalSSLEnable);

                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtpClient.Send(mailMsg);
                //ActivityLog.CreateActivityLog((Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSPrintSWPrincipal).UserKey : 0), null, null, DbConstants.LogType.Info,"","");
                ActivityLog.CreateActivityLog("EMAIL", "SEND", DbConstants.LogType.Info, DbConstants.User.UserKey, EduSuiteUIResources.Success);
                return true;
            }
            catch (SmtpException ex)
            {
                ActivityLog.CreateActivityLog("EMAIL", "SEND", DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                return false;
                //ActivityLog.CreateActivityLog((Thread.CurrentPrincipal.Identity.IsAuthenticated ? (Thread.CurrentPrincipal as CITSPrintSWPrincipal).UserKey : 0), null, null, DbConstants.LogType.Error, "", ex.GetBaseException().Message);
                // Code to Log error
            }
        }


    }
}