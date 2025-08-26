namespace Clinic_Backend.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Sex { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        // Questionnaire fields
        public string DesiredTreatment { get; set; } // hair, beard, eyebrows
        public string HairLossType { get; set; }
        public string HairColor { get; set; }
        public string HairLossDuration { get; set; }
        public bool HasPreviousTransplant { get; set; }
        public string PreferredTiming { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled

        public Clinic Clinic { get; set; }
        public ApplicationUser User { get; set; }
        public List<BookingImage> Images { get; set; } = [];
    }
}