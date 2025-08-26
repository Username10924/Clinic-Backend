namespace Clinic_Backend.DTOs
{
    public class CreateClinicDto
    {
        public string Name { get; set; }
        public string ClinicEmail { get; set; }
        public string Location { get; set; }
        public string ShortDescription { get; set; }
        public string AboutSection { get; set; }
        public decimal PriceRangeMin { get; set; }
        public decimal PriceRangeMax { get; set; }
        public int EstimatedAppointmentMonths { get; set; }
    }

    public class ClinicListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string ShortDescription { get; set; }
        public decimal PriceRangeMin { get; set; }
        public decimal PriceRangeMax { get; set; }
        public int EstimatedAppointmentMonths { get; set; }
        public double AverageRating { get; set; }
        public string BannerImagePath { get; set; }
    }

    public class ClinicDetailDto
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
        public List<string> Images { get; set; }
        public List<string> Videos { get; set; }
        public List<TreatmentDto> Treatments { get; set; }
        public List<DoctorDto> Doctors { get; set; }
        public List<ReviewDto> Reviews { get; set; }
        public int ReviewCount { get; set; }
    }

    public class TreatmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }

    public class DoctorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string ImagePath { get; set; }
    }
}