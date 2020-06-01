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
    public class PeopleController : ControllerBase
    {
        private ApplicationDbContext _context;
        public PeopleController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetPeople()
        {
            return Ok(
                _context.People.ToList()
                );
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetPerson(string id)
        {
            Person personInDb = _context.People.Single(p => p.Id == id);
            if (personInDb == null)
                return NotFound();
            return Ok(personInDb);
        }

        [HttpDelete]
        [Route("{Id}")]
        public IActionResult DeletePerson(string id)
        {
            Person personInDb = _context.People.Single(p => p.Id == id);
            if (personInDb == null)
                return NotFound();

            _context.People.Remove(personInDb);
            _context.SaveChanges();

            return Ok();
        }
    }
}