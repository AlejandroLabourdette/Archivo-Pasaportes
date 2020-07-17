using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Models
{
    public class PermanentTravel
    {
        public long Id { get; set; }

        [Required]
        public long TicketId { get; set; }
        public Ticket Ticket { get; set; }

        [Required]
        public long PassportId { get; set; }
        public Passport Passport { get; set; }

        [Required]
        public int OccupationId { get; set; }
        public Occupation Occupation { get; set; }
    }
}
