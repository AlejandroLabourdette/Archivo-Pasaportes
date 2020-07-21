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
            SeedSources();
            SeedPeople();
            SeedPassports();

            return Ok("Seed finish succefully.");
        }

        private void SeedSources()
        {
            AddSource(new Source()
            {
                Name = "Universidad de la Habana",
                Description = "Abarca toda la cuadra delimitada entre San Lázaro, Hotel Colina, Hospital Calixto García y el Ceder "
            });
            AddSource(new Source()
            {
                Name = "Ministerio de Relaciones Exteriores",
                Description = "Ubicado en G y 5, justo detrás del estdio José Martí"
            });

            _context.SaveChanges();
        }
        private void AddSource(Source source)
        {
            var sourceInDb = _context.Sources.SingleOrDefault(p => p.Name == source.Name);
            if (sourceInDb == null)
                _context.Sources.Add(source);
        }

        private void SeedPeople()
        {
            AddPerson(new Person()
            {
                CI = "85071623648",
                FirstName = "Ernesto",
                LastName = "Campos",
                BirthDay = new DateTime(1985, 7, 16),
                Address = "256, 23 e/ 12 y 14, Vedado, La Habana",
                SourceId = 1,
                Ocupation = "Profesor Titular"
            });
            AddPerson(new Person()
            {
                CI = "79081937689",
                FirstName = "Alberto",
                LastName = "Mesa",
                BirthDay = new DateTime(1979, 8, 19),
                Address = "801, 19 e/ D y E, Vedado, La Habana",
                SourceId = 2,
                Ocupation = "Jefe de Departamento de Atención al Reino Unido"
            });
            AddPerson(new Person()
            {
                CI = "01012487672",
                FirstName = "Luis",
                LastName = "Gonzalez",
                BirthDay = new DateTime(2001, 1, 24),
                Address = "405A, 23 e/ O y P, Vedado, La Habana",
                SourceId = 1,
                Ocupation = "Estudiante"
            });
            AddPerson(new Person()
            {
                CI = "76091928768",
                FirstName = "Cristóbal",
                LastName = "Mesa",
                BirthDay = new DateTime(1976, 9, 19),
                Address = "778, 21 e/ 8 y 10, Vedado, La Habana",
                SourceId = 1,
                Ocupation = "Decano de la Facultad de Contabilidad y Finanzas"
            });
            AddPerson(new Person()
            {
                CI = "89052756813",
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
            var personInDb = _context.People.SingleOrDefault(p => p.CI == person.CI);
            if (personInDb == null)
                _context.People.Add(person);
        }

        private void SeedPassports()
        {
            AddPassport(new Passport()
            {
                PassportTypeId = 2,
                PassportNo = "A879546",
                OwnerId = _context.People.Single(p=>p.CI=="85071623648").Id,
                SourceId = 1,
                ExpeditionDate = new DateTime(2018, 8, 5),
                ExpirationDate = new DateTime(2025, 4, 14)
            });
            AddPassport(new Passport()
            {
                PassportTypeId = 1,
                PassportNo = "A621549",
                OwnerId = _context.People.Single(p => p.CI == "79081937689").Id,
                SourceId = 2,
                ExpeditionDate = new DateTime(1992, 7, 27),
                ExpirationDate = new DateTime(2004, 6, 15)
            });
            AddPassport(new Passport()
            {
                PassportTypeId = 2,
                PassportNo = "A681672",
                OwnerId = _context.People.Single(p => p.CI == "79081937689").Id,
                SourceId = 2,
                ExpeditionDate = new DateTime(2005, 1, 17),
                ExpirationDate = new DateTime(2013, 9, 15)
            });
            AddPassport(new Passport()
            {
                PassportTypeId = 1,
                PassportNo = "A784568",
                OwnerId = _context.People.Single(p => p.CI == "79081937689").Id,
                SourceId = 2,
                ExpeditionDate = new DateTime(2015, 7, 1),
                ExpirationDate = new DateTime(2022, 2, 14)
            });
            AddPassport(new Passport()
            {
                PassportTypeId = 2,
                PassportNo = "A164827",
                OwnerId = _context.People.Single(p => p.CI == "01012487672").Id,
                SourceId = 1,
                ExpeditionDate = new DateTime(2017, 4, 4),
                ExpirationDate = new DateTime(2025, 9, 3)
            });
            AddPassport(new Passport()
            {
                PassportTypeId = 1,
                PassportNo = "A791528",
                OwnerId = _context.People.Single(p => p.CI == "76091928768").Id,
                SourceId = 1,
                ExpeditionDate = new DateTime(1999, 2, 28),
                ExpirationDate = new DateTime(2007, 6, 8)
            });
            AddPassport(new Passport()
            {
                PassportTypeId = 2,
                PassportNo = "A365284",
                OwnerId = _context.People.Single(p => p.CI == "76091928768").Id,
                SourceId = 1,
                ExpeditionDate = new DateTime(2009, 12, 10),
                ExpirationDate = new DateTime(2007, 11, 8)
            });
            _context.SaveChanges();
        }
        private void AddPassport(Passport passport)
        {
            var passportInDb = _context.Passports.SingleOrDefault(p => p.PassportNo == passport.PassportNo);
            if (passportInDb == null)
                _context.Passports.Add(passport);
        }
    }
}