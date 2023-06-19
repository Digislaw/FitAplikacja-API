using FitAplikacja.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitAplikacja.Core.Models
{
    public class Workout : IUserRelatedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Completed { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }

        public virtual ICollection<Exercise> Exercises { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
