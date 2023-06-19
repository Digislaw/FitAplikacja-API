using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FitAplikacja.Core.Models
{
    public class RefreshToken
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;

        [Required]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}
