namespace Clinic_Backend.Models
{
    public class BookingImage
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string ImagePath { get; set; }

        public Booking Booking { get; set; }
    }
}