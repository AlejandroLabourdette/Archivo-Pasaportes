using ArchivoDePasaportes.Models.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Models
{
    public class Person
    {
        [PersonIdValidation]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Carnet de Identidad")]
        public string Id { get; set; }


        [Required(ErrorMessage ="Por favor ingrese Nombre")]
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

        public int SourceId { get; set; }
        public Source Source { get; set; }

        public string Ocupation { get; set; }



        public string FullName()
        {
            return LastName+", "+FirstName;
        }
    }
}
