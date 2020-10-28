using ArchivoDePasaportes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.ViewModels
{
    public class DropPassportViewModel
    {
        public bool UserIsAdmin { get; set; }
        public DroppedPassport DroppedPassport { get; set; }
        public IEnumerable<DropCause> DropCauses { get; set; }
        public List<DroppedPassport> DroppedPassportsList { get; set; }
    }
}
