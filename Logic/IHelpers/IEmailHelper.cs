using Core.Models;

namespace Logic.IHelpers
{
	public interface IEmailHelper
	{
		Task<UserVerification> CreateUserToken(string userEmail);
		public bool PasswordResetLink(ApplicationUser applicationUser, string linkToClick);
		Task<UserVerification> GetUserToken(Guid token);
		Task<bool> MarkTokenAsUsed(UserVerification userVerification);
        void ConfirmationMessage(string email, string name);
        void PasswordResetConfirmation(string userEmail);
    }
}
