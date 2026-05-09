using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteerCenter.Models
{
    public class Volunteer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "ФИО обязательно")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "ФИО должно быть от 3 до 150 символов")]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string? Phone { get; set; }

        public DateTime? BirthDate { get; set; }

        public int? CuratorId { get; set; }

        public Specialist? Curator { get; set; }

        public ICollection<VolunteerBeneficiary> VolunteerBeneficiaries { get; set; } = new List<VolunteerBeneficiary>();
    }
}