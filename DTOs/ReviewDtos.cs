namespace Clinic_Backend.DTOs
{
    public class CreateReviewDto
    {
        public int ClinicId { get; set; }
        public bool IsAnonymous { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public int NumberOfGrafts { get; set; }
        public decimal Price { get; set; }
        public string TransplantType { get; set; }
        public int BookingToAppointmentDays { get; set; }
    }

    public class ReviewDto
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsAnonymous { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public int NumberOfGrafts { get; set; }
        public decimal Price { get; set; }
        public string TransplantType { get; set; }
        public int BookingToAppointmentDays { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Images { get; set; }
    }
}