using ArchivoDePasaportes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.ViewModels
{
    public class FlightViewModel
    {
        public bool UserIsAdmin { get; set; }
        public List<Ticket> TicketList { get; set; }
    }
}
