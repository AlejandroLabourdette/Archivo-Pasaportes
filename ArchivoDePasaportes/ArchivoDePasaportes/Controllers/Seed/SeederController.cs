using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Data;
using ArchivoDePasaportes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchivoDePasaportes.Controllers.Seed
{
    public class SeederController : Controller
    {
        private ApplicationDbContext _context;
        public SeederController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [Route("/Seed")]
        public IActionResult Seed()
        {
            SeedPeople();
            return Ok("Seed finish succefully.");
        }

        private void SeedPeople()
        {
            AddPerson(new Person()
            {
                Id = "85071623648",
                FirstName = "Ernesto",
                LastName = "Campos",
                BirthDay = new DateTime(1985, 7, 16),
                Address = "256, 23 e/ 12 y 14, Vedado, La Habana",
                SourceId = 1,
                Ocupation = "Profesor Titular"
            });
            AddPerson(new Person()
            {
                Id = "79081937689",
                FirstName = "Alberto",
                LastName = "Mesa",
                BirthDay = new DateTime(1979, 8, 19),
                Address = "801, 19 e/ D y E, Vedado, La Habana",
                SourceId = 2,
                Ocupation = "Jefe de Departamento de Atención al Reino Unido"
            });
            AddPerson(new Person()
            {
                Id = "01012487672",
                FirstName = "Luis",
                LastName = "Gonzalez",
                BirthDay = new DateTime(2001, 1, 24),
                Address = "405A, 23 e/ O y P, Vedado, La Habana",
                SourceId = 1,
                Ocupation = "Estudiante"
            });
            AddPerson(new Person()
            {
                Id = "76091928768",
                FirstName = "Cristóbal",
                LastName = "Mesa",
                BirthDay = new DateTime(1976, 9, 19),
                Address = "778, 21 e/ 8 y 10, Vedado, La Habana",
                SourceId = 1,
                Ocupation = "Decano de la Facultad de Contabilidad y Finanzas"
            });
            AddPerson(new Person()
            {
                Id = "89052756813",
                FirstName = "Marcos",
                LastName = "Rivera",
                BirthDay = new DateTime(1989, 5, 27),
                Address = "222, 17 e/ A y B, Vedado, La Habana",
                SourceId = 2,
                Ocupation = "Informático del Departamento de Tecnología"
            });
            _context.SaveChanges();
        }
        private void AddPerson(Person person)
        {
            var personInDb = _context.People.SingleOrDefault(p => p.Id == person.Id);
            if (personInDb == null)
                _context.People.Add(person);
        }
    }
}