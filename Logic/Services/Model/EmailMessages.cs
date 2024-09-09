using Microsoft.AspNetCore.Http;
using MimeKit;
using System.ComponentModel.DataAnnotations;

namespace Logic.Services.Model
{
    public class EmailMessage
    {
		public EmailMessage()
		{
			ToAddresses = new List<EmailAddress>();
		}

		[EmailAddress(ErrorMessage = "Invalid email address.")]
		public List<EmailAddress> ToAddresses { get; set; }
        /// <summary>
        /// The subject of the message
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The Main content of the message. Can be Html or string.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The media attachments to the message
        /// </summary>
        public IFormFileCollection Attachments { get; set; }
    }
}
