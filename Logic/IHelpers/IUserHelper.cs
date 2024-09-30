using Core.Models;
using Core.ViewModels;

namespace Logic.IHelpers
{
    public interface IUserHelper
    {
        Task<bool> CreateUser(ApplicationUserViewModel userDetails);
        Task<ApplicationUser>? FindByEmailAsync(string email);
        Task<ApplicationUser>? FindByPhoneNumber(string phone);
        string GetValidatedUrl();
    }
}
