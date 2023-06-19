using FitAplikacja.Core.Models.Services;
using FitAplikacja.Services.Abstract;
using FitAplikacja.Services.Settings;
using Google.Apis.Auth;
using System.Threading.Tasks;

namespace FitAplikacja.Services.Concrete
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly GoogleAuthSettings _googleAuthSettings;

        public GoogleAuthService(GoogleAuthSettings googleAuthSettings)
        {
            _googleAuthSettings = googleAuthSettings;
        }

        public async Task<GoogleTokenValidationResult> ValidateTokenAsync(string idToken)
        {
            GoogleJsonWebSignature.Payload payload;

            try
            {
                payload = await GoogleJsonWebSignature
                    .ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
                {
                        Audience = new[] { _googleAuthSettings.AppId }
                });
            }
            catch (System.Exception)
            {
                return new GoogleTokenValidationResult
                {
                    IsValid = false
                };
            }

            return new GoogleTokenValidationResult
            {
                IsValid = true,
                Email = payload.Email,
                Name = payload.GivenName.Split(' ')[0], // based on first names array
                PictureURL = payload.Picture
            };
        }
    }
}
