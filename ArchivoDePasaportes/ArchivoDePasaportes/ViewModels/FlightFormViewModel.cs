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

        public IList<PassInfoPermanentTravelDto> PermanentTravelsDto { get; set; }
        public IList<PassInfoOfficialTravelDto> OfficialTravelsDto { get; set; }

        public IEnumerable<Country> Countries{ get; set; }
        public IEnumerable<Occupation> Occupations { get; set; }

        public bool MissAnyPassport { get; set; }
        public bool RepetedPassport { get; set; }
        public bool ExistOtherTicketInDb { get; set; }
        public bool ReturnDateIncorrect { get; set; }
        public bool OcupationIncorrect { get; set; }

        public long OldTicketId { get; set; }
    }
}
