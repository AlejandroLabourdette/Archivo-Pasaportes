using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Models
{
    public class DropCause
    {
        public byte Id { get; set; }

        [Required(ErrorMessage = "Ingrese el nombre de la causa de deshecho")]
        [Display(Name = "Causa de deshecho")]
        public string Name { get; set; }
    }
}
