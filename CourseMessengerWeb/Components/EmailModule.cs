using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Elmah;

namespace CourseMessengerWeb.Components
{
    public class EmailModule
    {

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            if (sslpolicyerrors == SslPolicyErrors.None)
                return true;
            return true;
        }

        public void SendEmail(string emailAddress,string body,string subject)
        {
            try
            {

                var mail = new MailMessage();

                //recipients.ToList().ForEach(e => mail.To.Add(e));
                mail.To.Add(emailAddress);

                var bccs = ConfigurationManager.AppSettings["Mail:Bcc"];
                if (!string.IsNullOrEmpty(bccs))
                {
                    foreach (var bccRecipient in bccs.Split(','))
                    {
                        mail.Bcc.Add(bccRecipient);
                    }
                }


                mail.From = new MailAddress(ConfigurationManager.AppSettings["Mail:SenderAddress"], ConfigurationManager.AppSettings["Mail:SenderDisplayName"]);
                mail.Subject = subject;

                mail.Body = body;
                
                
                mail.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;

                    smtp.Send(mail);
                }


            }
            catch (System.ArgumentNullException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            catch (System.ArgumentException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            catch (System.FormatException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            catch (System.Net.Mail.SmtpFailedRecipientsException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            catch (System.ObjectDisposedException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            catch (System.InvalidOperationException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }



        }

      
    }
}