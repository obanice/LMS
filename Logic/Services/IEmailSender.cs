using Logic.Services.Model;
using Microsoft.AspNetCore.Http;

namespace Logic.Services
{
    public interface IEmailSender
    {


		/// <summary>
		/// Uses the default email configuration stored in app settings to send an email.
		/// </summary>
		/// <param name="toEmail"></param>
		/// <param name="subject"></param>
		/// <param name="message"></param>
		/// <param name="userDisplayName"></param>
		/// <remarks>Use this method when you want to send automated emails that do not require human interactions. Emails
		/// such as user registration emails, password recovery. These emails area usually from noreply@abc.com</remarks>
		void SendEmail(string toEmail, string subject, string message, string userDisplayName = null);

		/// <summary>
		/// Uses the default email configuration stored in app settings to send an email.
		/// </summary>
		/// <param name="toEmail"></param>
		/// <param name="subject"></param>
		/// <param name="message"></param>
		/// <param name="attachments"></param>
		/// <param name="userDisplayName"></param>
		/// /// <remarks>Use this method when you want to send automated emails that do not require human interactions. Emails
		/// such as user registration emails, password recovery. These emails area usually from noreply@abc.com</remarks>
		void SendEmailWithAttachment(string toEmail, string subject, string message, IFormFileCollection attachments, string userDisplayName = null);
	}
}