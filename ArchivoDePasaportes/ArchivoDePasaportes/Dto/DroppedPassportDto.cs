using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Dto
{
    public class DroppedPassportDto
    {
        [Display(Name = "Número de Pasaporte")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor ingrese el Número del Pasaporte")]
        public string PassportNo { get; set; }
        [Required(ErrorMessage = "Por favor ingrese el propietario del pasaporte")]
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
        [Required(ErrorMessage = "Por favor seleccione una Causa de Deshecho")]
        [Display(Name = "Causa de Deshecho")]
        public byte DropCauseId { get; set; }
        [Display(Name = "Detalles")]
        public string Details { get; set; }
    }
}
