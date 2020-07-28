using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Data;
using ArchivoDePasaportes.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArchivoDePasaportes.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DroppedPassportsController : ControllerBase
    {
        private ApplicationDbContext _context;
        public DroppedPassportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpDelete]
        [Route("{Id}")]
        public IActionResult DeletePassport(long id)
        {
            DroppedPassport passportInDb = _context.DroppedPassports.Single(p => p.Id == id);
            if (passportInDb == null)
                return NotFound();

            _context.DroppedPassports.Remove(passportInDb);
            _context.SaveChanges();

            return Ok();
        }
    }
}
