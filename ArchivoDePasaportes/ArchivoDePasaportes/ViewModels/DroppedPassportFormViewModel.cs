using ArchivoDePasaportes.Dto;
using ArchivoDePasaportes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.ViewModels
{
    public class DroppedPassportFormViewModel
    {
        public bool ExistOtherInDb { get; set; }
        public bool NotExistThisPersonInDb { get; set; }
        public string OldPassportNo { get; set; }
        public DroppedPassportDto DpDto { get; set; }
        public IEnumerable<DropCause> DropCauses { get; set; }
        public IEnumerable<PassportType> PassportTypes { get; set; }
        public IEnumerable<Source> Sources { get; set; }
    }
}
