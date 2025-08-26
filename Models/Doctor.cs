namespace Clinic_Backend.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string ImagePath { get; set; }

        public Clinic Clinic { get; set; }
    }
}