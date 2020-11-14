using System;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CalendrierAventCore.Tools
{
    public static class Tool
    {
        public static string RandomAsciiPrintable(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder stringBuilder = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint) * length];
                rng.GetBytes(uintBuffer);
                for (int i = 0; i < length; i++)
                {
                    uint num = BitConverter.ToUInt32(uintBuffer, sizeof(uint) * i);
                    stringBuilder.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }
            return stringBuilder.ToString();
        }

        public static string UrlVersNom(this string url)
        {
            return url.Replace("-", " ").Replace("_", "-");
        }
        public static string ToUrl(this string nom)
        {
            return nom.TrimEnd(' ').Replace("-", "_").Replace(" ", "-");
        }
        public static string NomAdmis(this string nom)
        {
            const char espace = ' ';
            const string interdit = "@=&#_;%^";
            return nom.Replace(interdit, espace).Replace(Path.GetInvalidFileNameChars(), espace);
        }
        public static string Replace(this string orig, string to, char by)
        {
            foreach (char car in to)
            {
                orig = orig.Replace(car, by);
            }
            return orig;
        }
        public static string Replace(this string orig, char[] to, char by)
        {
            foreach (char car in to)
            {
                orig = orig.Replace(car, by);
            }
            return orig;
        }

        public static bool EnvoieMail(string sendTo, string objectMail, string messageMail, MemoryStream attachment = null, string replyTo = "")
        {
            try
            {
                using (MailMessage message = new MailMessage())
                {
                    //string mailInfo = ConfigurationManager.AppSettings["MailInfo"];
                    const string mailInfo = "thomas@newtomsoft.fr";
                    //TODO mail
                    if (replyTo?.Length == 0)
                        replyTo = mailInfo;
                    message.From = new MailAddress(mailInfo);
                    message.To.Add(sendTo);
                    message.Subject = objectMail;
                    message.ReplyToList.Add(replyTo);
                    message.Body = messageMail;
                    message.IsBodyHtml = false;
                    if (attachment != null)
                    {
                        attachment.Position = 0;
                        Attachment data = new Attachment(attachment, "event.ics", "text/calendar");
                        message.Attachments.Add(data);
                    }
                    using SmtpClient client = new SmtpClient
                    {
                        EnableSsl = false
                    };
                    client.Send(message);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetHash(this string input)
        {
            using SHA256 hash = SHA256.Create();
            byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}