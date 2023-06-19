using Newtonsoft.Json;

namespace FitAplikacja.Core.Models.Facebook
{
    public class FacebookPicture
    {
        [JsonProperty("data")]
        public FacebookPictureData Data { get; set; }
    }
}
