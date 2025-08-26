namespace Clinic_Backend.Models
{
    public class Treatment
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public Clinic Clinic { get; set; }
    }
}