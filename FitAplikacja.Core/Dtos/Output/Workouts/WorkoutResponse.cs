using FitAplikacja.Core.Dtos.Output.Exercises;
using System;
using System.Collections.Generic;

namespace FitAplikacja.Core.Dtos.Output.Workouts
{
    public class WorkoutResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Completed { get; set; }
        public string ApplicationUserId { get; set; }
        public IEnumerable<ExerciseResponse> Exercises { get; set; }
    }
}
