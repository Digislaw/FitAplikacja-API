using FitAplikacja.Core.Models;
using FitAplikacja.Core.Models.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitAplikacjaAPI.Services.Abstract
{
    public interface IUserService
    {
        Task<ApplicationUser> GetByIdAsync(string id);
        Task<IEnumerable<ApplicationUser>> GetMany(int skip = 0, int take = 20);
        Task<IEnumerable<ApplicationUser>> SearchAsync(string email, string username);
        Task<bool> UpdateUser(ApplicationUser user);
        Task<bool> DeleteAsync(ApplicationUser user);


        Task<AuthenticationResult> RegisterAsync(string email, string username, string password);
        Task<AuthenticationResult> LoginAsync(string email, string password);
        Task<AuthenticationResult> AuthenticateFacebookAsync(string accessToken);
        Task<AuthenticationResult> AuthenticateGoogleAsync(string idToken);
        Task<AuthenticationResult> RefreshTokenAsync(string refreshTokenString);


        Task<IEnumerable<string>> GetUserRolesAsync(ApplicationUser user);
        Task<bool> SetUserRolesAsync(ApplicationUser user, IEnumerable<string> roles);
    }
}
