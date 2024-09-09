using Microsoft.AspNetCore.Http;
using MimeKit;
using MailKit.Net.Smtp;
using Logic.Services.Model;
using Microsoft.Extensions.Logging;
using Hangfire;
using MailKit.Security;
namespace Logic.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailMessage> _logger;
        private readonly IEmailConfiguration _emailConfig;
      
        public EmailSender(
            ILogger<EmailMessage> logger, 
            IEmailConfiguration emailConfiguration
            )
        {
            _logger = logger;
            _emailConfig = emailConfiguration;
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="toEmail"></param>
		/// <param name="subject"></param>
		/// <param name="message"></param>
		/// <param name="userDisplayName"></param>
		public void SendEmail(string toEmail, string subject, string message, string userDisplayName = null)
        {
			EmailAddress toAddress = new EmailAddress()
			{
				Address = toEmail
			};
			List<EmailAddress> toAddressList = new List<EmailAddress>
			{
					toAddress
			};
			EmailMessage emailMessage = new EmailMessage()
			{
				ToAddresses = toAddressList,
				Content = message,
				Subject = subject,
			};
			CallHangfire(emailMessage, [_emailConfig.SmtpUsername], _emailConfig.DisplayName);
        }
		/// <summary>
	    /// <inheritdoc/>
		/// </summary>
		/// <param name="toEmail"></param>
		/// <param name="subject"></param>
		/// <param name="message"></param>
		/// <param name="attachments"></param>
		/// <param name="userDisplayName"></param>
		public void SendEmailWithAttachment(string toEmail, string subject, string message, IFormFileCollection attachments, string userDisplayName = null)
		{
			EmailAddress toAddress = new EmailAddress()
			{
				Address = toEmail
			};
			List<EmailAddress> toAddressList = new List<EmailAddress>
			{
					toAddress
			};
			EmailMessage emailMessage = new EmailMessage()
			{
				ToAddresses = toAddressList,
				Content = message,
				Subject = subject,
                Attachments = attachments
			};
			CallHangfire(emailMessage, [_emailConfig.SmtpUsername], _emailConfig.DisplayName);
		}
		public void CallHangfire(EmailMessage mailMessage, string[] from, string displayUserName)
		{
            BackgroundJob.Enqueue(() => Send(mailMessage, from, displayUserName));
		}
		
		public void Send(EmailMessage message, string[] from, string displayUserName)
        {
            var emailMessage = new MimeMessage();
            string senderDisplayName = displayUserName ?? _emailConfig.DisplayName ?? "Email Message";
            foreach (string sender in from)
            {
                var ad = MailboxAddress.Parse(sender);
                ad.Name = senderDisplayName;
                emailMessage.From.Add(ad);
            }

			emailMessage.To.AddRange(message.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
			emailMessage.Subject = message.Subject;
            BodyBuilder emailBody;
            emailBody = BuildBody(message.Content, message.Attachments);
            emailMessage.Body = emailBody.ToMessageBody();
            using (var client = new SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.SmtpPort, SecureSocketOptions.Auto);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
					client.Authenticate(_emailConfig.SmtpUsername, _emailConfig.SmtpPassword);
					client.Send(emailMessage);
				}
                catch (Exception ex)
                {
                    _logger.LogError($"Message sending failed from {emailMessage.From} to {emailMessage.To}." +
                        $"{Environment.NewLine}{ex.Message}");
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        public BodyBuilder BuildBody(string message, IFormFileCollection attachments = null)
        {
            BodyBuilder body = new BodyBuilder
            {
                HtmlBody = message
            };

            if (attachments != null && attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    body.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }
            return body;
        }
    }
}
