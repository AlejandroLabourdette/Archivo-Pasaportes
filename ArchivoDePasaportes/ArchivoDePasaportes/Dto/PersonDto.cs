using ArchivoDePasaportes.Models.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Dto
{
    public class PersonDto
    {
        [PersonIdValidation]
        [Display(Name = "Carnet de Identidad")]
        public string CI { get; set; }


        [Required(ErrorMessage = "Por favor ingrese Nombre")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Por favor ingrese Apellidos")]
        [Display(Name = "Apellidos")]
        public string LastName { get; set; }


        [Display(Name = "Dirección")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Por favor ingrese un Fecha de Nacimiento")]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }


        [Display(Name = "Centro de Origen")]
        public int SourceId { get; set; }

        public string Ocupation { get; set; }
    }
}
