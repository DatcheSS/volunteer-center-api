namespace VolunteerCenter.Models
{
    public class VolunteerBeneficiary
    {
        public int VolunteerId { get; set; }
        public int BeneficiaryId { get; set; }

        public Volunteer Volunteer { get; set; } = null!;
        public Beneficiary Beneficiary { get; set; } = null!;

        public DateTime AssignmentDate { get; set; } = DateTime.UtcNow;
    }
}