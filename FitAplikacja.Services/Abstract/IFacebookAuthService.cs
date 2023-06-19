using FitAplikacja.Core.Dtos.Output.Facebook;
using System.Threading.Tasks;

namespace FitAplikacja.Services.Abstract
{
    public interface IFacebookAuthService
    {
        Task<FacebookTokenValidationResult> ValidateTokenAsync(string accessToken);
        Task<FacebookUserDataResult> GetUserDataAsync(string accessToken);
    }
}
