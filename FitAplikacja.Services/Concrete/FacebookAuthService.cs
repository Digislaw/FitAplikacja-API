using FitAplikacja.Core.Dtos.Output.Facebook;
using FitAplikacja.Services.Abstract;
using FitAplikacja.Services.Settings;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FitAplikacja.Services.Concrete
{
    public class FacebookAuthService : IFacebookAuthService
    {
        private const string TokenValidationUrl = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}|{2}";
        private const string UserDataUrl = "https://graph.facebook.com/me?fields=first_name,last_name,picture,email&access_token={0}";
        
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly FacebookAuthSettings _facebookAuthSettings;

        public FacebookAuthService(IHttpClientFactory httpClientFactory, FacebookAuthSettings facebookAuthSettings)
        {
            _httpClientFactory = httpClientFactory;
            _facebookAuthSettings = facebookAuthSettings;
        }

        public async Task<FacebookUserDataResult> GetUserDataAsync(string accessToken)
        {
            var userDataUrl = string.Format(UserDataUrl, accessToken);

            var result = await _httpClientFactory.CreateClient().GetAsync(userDataUrl);
            result.EnsureSuccessStatusCode();

            var responseString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookUserDataResult>(responseString);
        }

        public async Task<FacebookTokenValidationResult> ValidateTokenAsync(string accessToken)
        {
            var validationUrl = string.Format(TokenValidationUrl, accessToken, _facebookAuthSettings.AppId, _facebookAuthSettings.AppSecret);

            var result = await _httpClientFactory.CreateClient().GetAsync(validationUrl);
            result.EnsureSuccessStatusCode();

            var responseString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookTokenValidationResult>(responseString);
        }
    }
}
