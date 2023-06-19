namespace FitAplikacja.Core.Models.Services
{
    public class GoogleTokenValidationResult
    {
        public bool IsValid { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PictureURL { get; set; }
    }
}
