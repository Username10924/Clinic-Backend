namespace Clinic_Backend.Models
{
    public class ClinicVideo
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }
        public string VideoPath { get; set; }

        public Clinic Clinic { get; set; }
    }
}