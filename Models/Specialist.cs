using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VolunteerCenter.Models


{
    public class Specialist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string? Phone { get; set; }

        [Required]
        public string Position { get; set; } = null!;  

        [JsonIgnore]
        public ICollection<Volunteer> Volunteers { get; set; } = new List<Volunteer>();
    }
}