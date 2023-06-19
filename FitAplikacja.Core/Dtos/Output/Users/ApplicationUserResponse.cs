using System;

namespace FitAplikacja.Core.Dtos.Output.Users
{
    /// <summary>
    /// Output for admin
    /// </summary>
    public class ApplicationUserResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTime LockoutEnd { get; set; }

    }
}
