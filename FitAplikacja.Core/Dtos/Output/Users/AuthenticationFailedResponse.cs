using System.Collections.Generic;

namespace FitAplikacja.Core.Dtos.Output.Users
{
    public class AuthenticationFailedResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}
