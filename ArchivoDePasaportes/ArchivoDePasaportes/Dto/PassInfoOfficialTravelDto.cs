using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Dto
{
    public class PassInfoOfficialTravelDto
    {
        public string PassportNo { get; set; }
        public int OcupationId { get; set; }
        public DateTime ReturnDate { get; set; }

    }
}
