using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;


namespace MailingService
{
    class Email
    {
        #region Send Email Code Function  
        public bool SendEmail(KeyValuePair<String, String> ToMail, List<KeyValuePair<String, String>> Cc, String Subj, String Message, List<String> attachments)
        {
            string HostAdd = ConfigurationManager.AppSettings["Host"].ToString();
            string FromEmailID = ConfigurationManager.AppSettings["FromEmailID"].ToString();
            string PasswordMail = ConfigurationManager.AppSettings["PasswordMail"].ToString();
            
            //creating the object of MailMessage  

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Hostel Attendance IIT Mandi", FromEmailID));
            message.To.Add(new MailboxAddress(ToMail.Key, ToMail.Value));
            foreach (KeyValuePair<String, String> PSS in Cc)
            {
                message.Cc.Add(new MailboxAddress(PSS.Key, PSS.Value));
            }
            message.Subject = Subj;

            var builder = new BodyBuilder
            {
                TextBody = Message
            };
            foreach (String attach in attachments)
            {
                builder.Attachments.Add(attach);
            }
                

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect(HostAdd, 587);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(FromEmailID, PasswordMail);

                try
                {
                    client.Send(message);
                    Console.WriteLine("Email sent to : " + ToMail.Value);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    client.Disconnect(true);
                    return false;
                }
                client.Disconnect(true);
            }
            return true;
        }

        #endregion

        #region Send Email to Absent Student
        public void SendEmailTOAbsenties(List<List<String>> absent,int dayNo)
        {

            string FromEmailID = ConfigurationManager.AppSettings["FromEmailID"].ToString();

            String message = "Dear Student \n" +
                "You have not marked your today's hostel attendance yet. \n" +
                "Date : " + DateTime.Now.AddDays(dayNo).ToString("dd-MM-yyyy") + "\n" +
                "Do mark your attendance in your Hostel's Biometric Machine before 12:30 mid night \n\n" +
                "If you are unable to mark your presence in the hostel, write an email to your caretaker and put your warden in cc " +
                "with a valid reason. \n" +
                "It need not to have permission but It is an necessary intimation as per hostel rules. \n\n" +

                "If you have already mark your attendance, \n" +
                "kindly inform to your caretaker, mark again, notice your roll no and time in machine, pass this information to your caretaker. \n\n" +
                "Note : If you fail to follow the procedure, fine will be auto genrated. \n" +
                "Thanks\n" ;

            foreach(List<String> absstudent in absent)
            {
                KeyValuePair<String, String> toMail = new KeyValuePair<string, string>(absstudent[1], absstudent[6]);
                String subj = "Mark Your Attendance Date: " + DateTime.Now.AddDays(dayNo).ToString("dd-MM-yyyy");
                KeyValuePair<String, String> kss = new KeyValuePair<String, String >("Hostel Attendance IIT Mandi", FromEmailID);
                List<KeyValuePair<String, String>> cc = new List<KeyValuePair<string, string>>();
                this.SendEmail(toMail, cc, subj, message, new List<string>());
            }
        }
        #endregion

        #region Send Email to Caretaker and Warden's 
        public void SendEmailTOCaretakerWardens(KeyValuePair<String, String> toMail, List<KeyValuePair<String, String>> cc, List<String> attachments, List<List<String>> absents, string hostel, int dayNo)
        {
            String message = "Dear Sir/Mam \n" +
                "Find the attachments. \n" +
                "One of them contain the list of students who did not mark their yesterday's attendance in Biometric machine. \n" +
                "Except those all are fine. \n" +
                "kindly resolve the same. \n\n" +
                "Thanks\n";

            foreach (List<String> absstudent in absents)
            {
                KeyValuePair<String, String> kss = new KeyValuePair<String, String>(absstudent[1], absstudent[6]);
                cc.Add(kss);
            }
            this.SendEmail(toMail, cc, hostel + " hostel Attendance Dated :" + DateTime.Now.AddDays(dayNo).ToString("dd-MM-yyyy"), message, attachments);
        }
        #endregion
    }
}
