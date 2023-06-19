using FitAplikacja.Core.Interfaces;
using System.Collections.Generic;

namespace FitAplikacja.Core.Models
{
    public class Exercise : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double BurnedCalories { get; set; }
        public bool? IsWeightTraining { get; set; }
        public byte Difficulty { get; set; }
        public string VideoURL { get; set; }
        public string Description { get; set; }
        public byte Series { get; set; }
        public int Weight { get; set; }
        public string BodyPart { get; set; }
        public byte Repetition { get; set; }


        public virtual ICollection<Workout> Workouts { get; set; }
    }
}
