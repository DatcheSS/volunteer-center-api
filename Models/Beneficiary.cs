using System.ComponentModel.DataAnnotations;

namespace VolunteerCenter.Models
{
    public class Beneficiary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;
        
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string? Phone { get; set; }

        public string? Description { get; set; } 

        public ICollection<VolunteerBeneficiary> VolunteerBeneficiaries { get; set; } = new List<VolunteerBeneficiary>();
    }
}