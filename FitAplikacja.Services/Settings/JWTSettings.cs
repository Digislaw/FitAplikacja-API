namespace FitAplikacjaAPI.Settings
{
    public class JWTSettings
    {
        /// <summary>
        /// Secret key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Audience claim ("aud")
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Issuer claim ("iss")
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Time in minutes after JWT will not be accepted
        /// </summary>
        public double Duration { get; set; }

    }
}
