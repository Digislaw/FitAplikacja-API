using System;

namespace FitAplikacja.Core.Dtos.Input.Workouts
{
    public class WorkoutInputRequest
    {
        public string Name { get; set; }
        public DateTime? Completed { get; set; }
    }
}
