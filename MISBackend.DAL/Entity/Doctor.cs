using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MISBackend.DAL.Entity
{
    [Table("Doctor")]
    public class Doctor
    {
        [Key]
        public Guid Id { get; set; }
        [Required, StringLength(100)]
        public required string Name { get; set; }
        [Required, StringLength(255)]
        public required string Password { get; set; }
        [Required, StringLength(255)]
        public required string Email { get; set; }
        [Required]
        public DateTime Birthday { get; set; }
        [Required]
        public Enum.DataEnum.Gender Gender { get; set; }
        [Required, StringLength(100)]
        public required string Phone { get; set; }
        [Required]
        public Guid Speciality { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
