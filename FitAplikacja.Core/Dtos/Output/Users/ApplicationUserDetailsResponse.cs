namespace FitAplikacja.Core.Dtos.Output.Users
{
    /// <summary>
    /// Application data of user
    /// </summary>
    public class ApplicationUserDetailsResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int? Weight { get; set; }
        public int? Height { get; set; }
        public int? TargetWeight { get; set; }
        public int? Age { get; set; }
        public int? Kcal { get; set; }
    }
}
