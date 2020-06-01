using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Models
{
    public class PassportType
    {
        public byte Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese el nombre para el nuevo tipo de Pasaportes")]
        [Display(Name = "Tipo de Pasaporte")]
        public string Name { get; set; }
    }
}
