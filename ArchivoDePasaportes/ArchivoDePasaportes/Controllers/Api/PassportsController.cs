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
    public class PassportsController : ControllerBase
    {
        private ApplicationDbContext _context;
        public PassportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpDelete]
        [Route("{Id}")]
        public IActionResult DeletePassport(long id)
        {
            Passport passportInDb = _context.Passports.Single(p => p.Id == id);
            if (passportInDb == null)
                return NotFound();

            _context.Passports.Remove(passportInDb);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPut]
        [Route("{Id}")]
        public IActionResult ArchivePassport(long id)
        {
            Passport passportInDb = _context.Passports.SingleOrDefault(p => p.Id == id);
            if (passportInDb == null)
                return NotFound();
            var givePassportsActive = from gp in _context.GivePassports where gp.Active select gp; 
            var giveLog = givePassportsActive.SingleOrDefault(gp => gp.PassportId == passportInDb.Id);
            if (giveLog == null)
                return BadRequest();

            passportInDb.IsPassportArchived = true;
            giveLog.Active = false;
            _context.SaveChanges();

            return Ok();
        }
    }
}