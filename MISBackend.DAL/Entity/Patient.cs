using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MISBackend.DAL.Entity
{
    [Table("Patient")]
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }
        [Required, StringLength(255)]
        public required string Name { get; set; }
        [Required]
        public DateTime Birthday { get; set; }
        [Required]
        public Enum.DataEnum.Gender Gender { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
