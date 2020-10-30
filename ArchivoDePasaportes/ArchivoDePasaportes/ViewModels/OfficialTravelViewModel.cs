using ArchivoDePasaportes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.ViewModels
{
    public class OfficialTravelViewModel
    {
        public List<OfficialTravel> OfficialTravelsList { get; set; }
        public bool UserIsAdmin { get; set; }
    }
}
