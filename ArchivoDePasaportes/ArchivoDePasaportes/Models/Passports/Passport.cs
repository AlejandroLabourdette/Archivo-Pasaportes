using ArchivoDePasaportes.Models.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Models
{
    public class Passport
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Número de Pasaporte")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Id { get; set; }


        [PersonIdValidation]
        [Required(ErrorMessage = "Por favor ingrese el propietario del pasaporte")]
        public string OwnerId { get; set; }
        public Person Owner { get; set; }

        
        [Required(ErrorMessage = "Por favor seleccione el tipo de este pasaporte")]
        [Display(Name = "Tipo de Pasaportes")]
        public byte PassportTypeId { get; set; }
        public PassportType PassportType { get; set; }

        
        [Display(Name = "Centro Tramitador")]
        public int? SourceId { get; set; }
        public Source Source { get; set; }
        
        
        [Required]
        [Display(Name = "Fecha de Expedición")]
        [DataType(DataType.Date)]
        public DateTime ExpeditionDate { get; set; }

        
        [Required]
        [Display(Name = "Fecha de Vencimiento")]
        [DataType(DataType.Date)]
        public DateTime ExpirationDate { get; set; }
    }
}
