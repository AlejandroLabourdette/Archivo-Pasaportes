using ArchivoDePasaportes.Dto;
using ArchivoDePasaportes.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.ViewModels
{
    public class FlightFormViewModel
    {
        public Ticket Ticket { get; set; }

        public bool HasReturnDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; set; }
        public IList<PassportToTravelDto> PassportsToTravelDto { get; set; }

        public IEnumerable<Country> Countries{ get; set; }
        public IEnumerable<Occupation> Occupations { get; set; }
    }
}
