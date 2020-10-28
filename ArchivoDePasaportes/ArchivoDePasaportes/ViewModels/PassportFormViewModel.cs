using ArchivoDePasaportes.Dto;
using ArchivoDePasaportes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.ViewModels
{
    public class PassportFormViewModel
    {
        public bool ExistOtherInDb { get; set; }
        public bool NotExistThisPersonInDb { get; set; }
        public bool ValidDates { get; set; }
        public bool UserIsAdmin { get; set; }
        public string OldPassportNo { get; set; }
        public PassportDto PassportDto { get; set; }
        public IEnumerable<Source> Sources { get; set; }
        public IEnumerable<PassportType> PassportTypes { get; set; }
        public List<Passport> PassportList { get; set; }
    }
}
