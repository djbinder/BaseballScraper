using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using C = System.Console;

#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0063, IDE1006
namespace BaseballScraper.Infrastructure
{
    public class EmailHelper
    {

        private readonly Helpers        _helpers;
        private readonly IConfiguration _configuration;
        private readonly string         _gmail1Value;
        private readonly string         _gmail1PasswordForAppValue;

        public EmailHelper(Helpers helpers, IConfiguration configuration)
        {
            _helpers = helpers;
            _configuration = configuration;
            _gmail1Value = _configuration.GetSection("Gmail1").Value;
            _gmail1PasswordForAppValue = _configuration.GetSection("Gmail1_Password_App_Access").Value;
        }


        public EmailHelper(){}



        // STATUS [ July 16, 2019 ] : this works
        /// <summary>
        ///     Send an email
        /// </summary>
        /// <param name="emailSubject">todo: describe emailSubject parameter on SendEmail</param>
        /// <param name="emailBodyContent">todo: describe emailBodyContent parameter on SendEmail</param>
        /// <remarks>
        ///     _gmail1Value and _gmail1PasswordForAppValue:
        ///         * Are set in user secrets
        ///         * Are configured in Startup.cs
        ///     _gmail1Value:
        ///         * Is not primary email address
        ///     _gmail1PasswordForAppValue is :
        ///         * configured via: https://myaccount.google.com/u/1/apppasswords
        ///         * Is a PW from gmail account that only works for this app
        ///         * Consider it an alternative to normal password
        ///     See:
        ///         * https://garrymarsland.com/sending-email-from-a-net-core-2-1/
        ///         * https://support.google.com/accounts/answer/185833
        ///         * Google Account > Security > Signing in to Google > App passwords
        /// </remarks>
        /// <example>
        ///     _emailHelper.SendEmail("TEST2", "TESTING SOMETHING NEW");
        /// </example>
        public void SendEmail(string emailSubject, string emailBodyContent)
        {
            using (MailMessage message = new MailMessage())
            {
                message.To.Add(new MailAddress(_gmail1Value, "RECEIVER"));
                message.From = new MailAddress(_gmail1Value, "Baseball Scraper Reporting");

                message.Subject    = emailSubject;
                message.Body       = emailBodyContent;
                message.IsBodyHtml = true;

                using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
                {
                    client.Port        = 587;
                    client.Credentials = new NetworkCredential(_gmail1Value, _gmail1PasswordForAppValue);
                    client.EnableSsl   = true;

                    try
                    {
                        client.Send(message);
                        C.WriteLine("Email Successfully Sent!");
                    }

                    catch(Exception ex)
                    {
                        C.WriteLine("Email Failed to Send");
                        C.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}
