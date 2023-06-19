using System.Collections.Generic;

namespace FitAplikacja.Core.Models.Services
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public string Token { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public string UserId { get; set; }

    }
}
