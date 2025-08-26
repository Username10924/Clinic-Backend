namespace Clinic_Backend.Models
{
    public class Clinic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClinicEmail { get; set; }
        public string Location { get; set; }
        public string ShortDescription { get; set; }
        public string AboutSection { get; set; }
        public decimal PriceRangeMin { get; set; }
        public decimal PriceRangeMax { get; set; }
        public int EstimatedAppointmentMonths { get; set; }
        public double AverageRating { get; set; }
        public string BannerImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<ClinicImage> Images { get; set; } = [];
        public List<ClinicVideo> Videos { get; set; } = [];
        public List<Treatment> Treatments { get; set; } = [];
        public List<Doctor> Doctors { get; set; } = [];
        public List<Review> Reviews { get; set; } = [];
        public List<Booking> Bookings { get; set; } = [];
    }
}