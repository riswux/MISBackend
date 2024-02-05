using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MISBackend.Model.Payload
{
    public class DoctorEditModel
    {
        [Required, StringLength(255)]
        public required string Email { get; set; }
        public required string Name { get; set; }
        [Required]
        public DateTime Birthday { get; set; }
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))] // Anotasi untuk menggunakan JsonStringEnumConverter
        public Enum.DataEnum.Gender Gender { get; set; }
        [Required, StringLength(100)]
        public required string Phone { get; set; }
    }
}
