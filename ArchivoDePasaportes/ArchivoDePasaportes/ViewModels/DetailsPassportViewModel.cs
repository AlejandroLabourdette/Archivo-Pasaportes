using ArchivoDePasaportes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.ViewModels
{
    public class DetailsPassportViewModel
    {
        public Passport Passport { get; set; }
        public List<GivePassport> GivePassports { get; set; }
    }
}
