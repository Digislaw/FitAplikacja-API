using FitAplikacja.Core.Models.Services;
using System.Threading.Tasks;

namespace FitAplikacja.Services.Abstract
{
    public interface IGoogleAuthService
    {
        Task<GoogleTokenValidationResult> ValidateTokenAsync(string idToken);
    }
}
