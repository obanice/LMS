﻿using Core.Db;
using Core.Models;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.EntityFrameworkCore;

namespace Logic.Helpers
{
	public class EmailHelper : IEmailHelper
	{
		private readonly IUserHelper _userHelper;
		private readonly IEmailSender _emailService;
		private readonly AppDbContext _context;
		public EmailHelper(AppDbContext context,
			IUserHelper userHelper, 
			IEmailSender emailService)
		{
			_context = context;
			_emailService = emailService;
			_userHelper = userHelper;
		}


		public async Task<UserVerification> CreateUserToken(string userEmail)
		{
			var user = await _userHelper.FindByEmailAsync(userEmail);
			if (user == null)
			{
				return null;
			}
			UserVerification userVerification = new UserVerification()
			{
				UserId = user.Id,
			};
			await _context.AddAsync(userVerification);
			await _context.SaveChangesAsync();
			return userVerification;
		}

		public async Task<UserVerification> GetUserToken(Guid token)
		{
			return await _context.UserVerifications.Where(t => t.Used != true && t.Token == token)?.Include(s => s.User).FirstOrDefaultAsync();
		}

		public bool PasswordResetLink(ApplicationUser applicationUser, string linkToClick)
		{
			string toEmail = applicationUser.Email;
			string subject = "LMS Support";
			string message = "Hi  " + applicationUser.FirstName + "<br/> A new account with password 12345 and email " + applicationUser.Email + " has been created for you on LMS software by the admin. " +
							"<br/> Please click on the button below to create a new password <br>" +
							"<br/>" + "<a style:'border:2px;' href='" + linkToClick + "' target='_blank'>" + "<button style='color:white; background-color:#366092; padding:10px; border:1px;'>Reset Password</button>" + "</a>" +
							"<br/> or copy the link below to your browser </br>" + linkToClick + "<br/>" +
							"</br> Need help ? We’re here for you, simply reply to this email to contact us. <br/>" +
							"<br/>" +
							"<b>Kind regards,</b><br/>" +
							"<b>LMS Software</b>";
			_emailService.SendEmail(toEmail, subject, message);
			return true;
		}
        public void ConfirmationMessage(string email, string name)
        {
            string subject = "LMS Support";
            string message = "Hi " + name + ",<br/><br/>" +
							"Welcome to LMS Software! We are excited to have you on board.<br/><br/>" +
							"Your account has been successfully created, and you're all set to explore the platform.<br/><br/>" +
							"If you have any questions or need assistance, feel free to reach out by replying to this email. We're here to help!<br/><br/>" +
							"Best regards,<br/>" +
							"<b>LMS Software Team</b>";

            _emailService.SendEmail(email, subject, message);
        }

		public void PasswordResetConfirmation(string userEmail)
		{
			if (string.IsNullOrEmpty(userEmail))
			{
				return;
			}
			string subject = "Password Reset Successfully";
			string message = "Password has been reset successfully, login with your new details to continue" + "</br>" +
				"<br/>Need help? We’re here for you." + "<br/>" +
				"Simply reply to this email to contact us. <br/>" +
				"<br/>" +
				"Kind regards,<br/>";

			_emailService.SendEmail(userEmail, subject, message);
		}
        public async Task<bool> MarkTokenAsUsed(UserVerification userVerification)
		{
			var VerifiedUser = _context.UserVerifications.FirstOrDefault(s => s.UserId == userVerification.User.Id && !s.Used);
			if (VerifiedUser == null)
			{
				return false;
			}
			userVerification.Used = true;
			userVerification.DateUsed = DateTime.Now;
			_context.Update(userVerification);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
