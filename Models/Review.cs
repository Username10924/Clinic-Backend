namespace Clinic_Backend.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }
        public string UserId { get; set; }
        public bool IsAnonymous { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public int NumberOfGrafts { get; set; }
        public decimal Price { get; set; }
        public string TransplantType { get; set; } // e.g., "FUE", "FUT"
        public int BookingToAppointmentDays { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Clinic Clinic { get; set; }
        public ApplicationUser User { get; set; }
        public List<ReviewImage> Images { get; set; } = [];
    }
}