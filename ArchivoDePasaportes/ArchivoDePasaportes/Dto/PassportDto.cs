using ArchivoDePasaportes.Models;
using ArchivoDePasaportes.Models.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Dto
{
    public class PassportDto
    {
        [Display(Name = "Número de Pasaporte")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor ingrese el Número del Pasaporte")]
        public string PassportNo { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor ingrese el CI del propietario")]
        [Display(Name = "Carnet de Identidad del Propietario")]
        public string OwnerCI { get; set; }


        [Required(ErrorMessage = "Por favor seleccione el tipo de este pasaporte")]
        [Display(Name = "Tipo de Pasaporte")]
        public byte PassportTypeId { get; set; }


        [Display(Name = "Centro Tramitador")]
        public int? SourceId { get; set; }


        [Required(ErrorMessage = "Por favor ingrese una Fecha de Expedición")]
        [Display(Name = "Fecha de Expedición")]
        [DataType(DataType.Date)]
        public DateTime? ExpeditionDate { get; set; }


        [Required(ErrorMessage = "Por favor ingrese una Fecha de Vencimiento")]
        [Display(Name = "Fecha de Vencimiento")]
        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }

        [Display(Name = "Pasaporte Archivado")]
        public bool IsPassportArchived { get; set; }
    }
}
