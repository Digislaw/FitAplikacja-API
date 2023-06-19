using FitAplikacja.Core.Models.Facebook;
using Newtonsoft.Json;

namespace FitAplikacja.Core.Dtos.Output.Facebook
{
    public class FacebookUserDataResult
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("picture")]
        public FacebookPicture FacebookPicture { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
