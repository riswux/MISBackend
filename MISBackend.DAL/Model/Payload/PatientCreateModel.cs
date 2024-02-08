using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MISBackend.DAL.Model.Payload
{
    public class PatientCreateModel
    {
        [Required, StringLength(255)]
        public required string Name { get; set; }
        [Required]
        public DateTime Birthday { get; set; }
        [Required]
        public Enum.DataEnum.Gender Gender { get; set; }
    }
}
