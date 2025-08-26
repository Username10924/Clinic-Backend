namespace Clinic_Backend.Models
{
    public class SiteContent
    {
        public int Id { get; set; }
        public string ContentType { get; set; } // TOS, FAQ, PrivacyPolicy.
        public string Content { get; set; } // Could be plain text or HTML.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}