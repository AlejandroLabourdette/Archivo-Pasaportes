using ArchivoDePasaportes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.ViewModels
{
    public class DropPassportViewModel
    {
        public DroppedPassport DroppedPassport { get; set; }
        public IEnumerable<DropCause> DropCauses { get; set; }
    }
}
