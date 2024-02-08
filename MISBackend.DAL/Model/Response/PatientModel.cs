using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISBackend.DAL.Model.Response
{
    public class PatientModel
    {
        public Guid Id { get; set; }
        [Required, StringLength(255)]
        public required string Name { get; set; }
        [Required]
        public DateTime Birthday { get; set; }
        public Enum.DataEnum.Gender Gender { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
