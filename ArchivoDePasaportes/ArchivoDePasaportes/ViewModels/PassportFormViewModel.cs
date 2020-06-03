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
        public string OldId { get; set; }
        public Passport Passport { get; set; }
        public IEnumerable<Source> Sources { get; set; }
        public IEnumerable<PassportType> PassportTypes { get; set; }
    }
}
