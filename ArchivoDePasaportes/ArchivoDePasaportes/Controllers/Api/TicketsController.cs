using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArchivoDePasaportes.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        public ApplicationDbContext _context { get; set; }
        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpDelete]
        [Route("{Id}")]
        public IActionResult Delete(long id)
        {
            var ticketInDb = _context.Tickets.SingleOrDefault(t => t.Id == id);
            if (ticketInDb == null)
                return NotFound();

            _context.Tickets.Remove(ticketInDb);
            _context.SaveChanges();

            return Ok();
        }
    }
}