using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Models
{
    public class GivePassport
    {
        public long Id { get; set; }
        [Required]
        public long PassportId { get; set; }
        public Passport Passport { get; set; }
        [Required]
        public DateTime GiveDate { get; set; }
        public DateTime? ExpectedReturn { get; set; }
        public string Description { get; set; }
    }
}
