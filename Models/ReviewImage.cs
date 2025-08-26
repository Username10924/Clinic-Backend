namespace Clinic_Backend.Models
{
    public class ReviewImage
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string ImagePath { get; set; }

        public Review Review { get; set; }
    }
}