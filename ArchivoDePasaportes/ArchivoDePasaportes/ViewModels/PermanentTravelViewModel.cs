using ArchivoDePasaportes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.ViewModels
{
    public class PermanentTravelViewModel
    {
        public bool UserIsAdmin { get; set; }
        public List<PermanentTravel> PermanentTravelsList { get; set; }
    }
}
