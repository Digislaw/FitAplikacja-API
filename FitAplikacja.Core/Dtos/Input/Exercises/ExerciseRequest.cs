namespace FitAplikacja.Core.Dtos.Input.Exercises
{
    public class ExerciseRequest
    {
        public string Name { get; set; }
        public double BurnedCalories { get; set; }
        public bool IsWeightTraining { get; set; }
        public byte Difficulty { get; set; }
        public string VideoURL { get; set; }
        public string Description { get; set; }
        public byte Series { get; set; }
        public int Weight { get; set; }
        public string BodyPart { get; set; }
        public byte Repetition { get; set; }
    }
}
