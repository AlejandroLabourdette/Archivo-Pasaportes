using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Dto
{
    public class GivePassportDto
    {
        [Required]
        public string PassportNo { get; set; }
        [Required]
        public DateTime GiveDate { get; set; }
        public DateTime? ExpectedReturn { get; set; }
        public string Description { get; set; }
    }
}
