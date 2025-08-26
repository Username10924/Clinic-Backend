namespace Clinic_Backend.DTOs
{
    public class CreateBookingDto
    {
        public int ClinicId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Sex { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string DesiredTreatment { get; set; }
        public string HairLossType { get; set; }
        public string HairColor { get; set; }
        public string HairLossDuration { get; set; }
        public bool HasPreviousTransplant { get; set; }
        public string PreferredTiming { get; set; }
    }

    public class BookingDto
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }
        public string ClinicName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Sex { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string DesiredTreatment { get; set; }
        public string HairLossType { get; set; }
        public string HairColor { get; set; }
        public string HairLossDuration { get; set; }
        public bool HasPreviousTransplant { get; set; }
        public string PreferredTiming { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public List<string> Images { get; set; }
    }
}