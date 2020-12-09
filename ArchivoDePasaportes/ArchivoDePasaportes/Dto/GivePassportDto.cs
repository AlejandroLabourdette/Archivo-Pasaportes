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
        [Display(Name ="Número de Pasaporte")]
        public string PassportNo { get; set; }
        [Required]
        [Display(Name ="Fecha de Entrega")]
        public DateTime GiveDate { get; set; }
        [Display(Name ="Fecha de Devolución")]
        public DateTime? ExpectedReturn { get; set; }
        [Display(Name ="Descripción")]
        public string Description { get; set; }
    }
}
