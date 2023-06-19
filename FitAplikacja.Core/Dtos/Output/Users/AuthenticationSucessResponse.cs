using System;

namespace FitAplikacja.Core.Dtos.Output.Users
{
    public class AuthenticationSucessResponse
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

    }
}
