using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FitAplikacja.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? Weight { get; set; }
        public int? Height { get; set; }
        public int? TargetWeight { get; set; }
        public int? Age { get; set; }
        public int? Kcal { get; set; }

        [JsonIgnore]
        public virtual RefreshToken RefreshToken { get; set; }
        public virtual ICollection<Workout> Workouts { get; set; }
        public virtual ICollection<AssignedProduct> AssignedProduct { get; set; }
    }
}
