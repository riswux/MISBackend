using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MISBackend.DAL.Model.Response
{
    public class DoctorModel
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        [Required, StringLength(100)]
        public required string Name { get; set; }
        [Required]
        public DateTime Birthday { get; set; }
        [Required]
        public Enum.DataEnum.Gender Gender { get; set; }
        [Required, StringLength(255)]
        public required string Email { get; set; }
        [Required, StringLength(100)]
        public required string Phone { get; set; }
    }
}
