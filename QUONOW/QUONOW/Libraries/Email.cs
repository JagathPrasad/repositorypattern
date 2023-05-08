using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QUONOW.Libraries
{
    public class Email
    {

        #region Properties

        public string From { get; set; }

        public string To { get; set; }

        public string Attachment { get; set; }


        public string Subject { get; set; }

        public string Body { get; set; }






        #endregion

        #region Global Declarations
        private string FromAddress = "info@quonow.com";

        #endregion
        public bool SendEmail()
        {
            bool IsSend = false;
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                msg.From = new System.Net.Mail.MailAddress(string.IsNullOrEmpty(this.From) ? this.FromAddress : this.From);
                msg.To.Add(this.To);
                msg.IsBodyHtml = true;
                msg.Subject = this.Subject;
                msg.Body = this.Body;
                using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient())
                {
                    smtp.Send(msg);
                    IsSend = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                IsSend = false;
            }
            return IsSend;
        }

    }
}