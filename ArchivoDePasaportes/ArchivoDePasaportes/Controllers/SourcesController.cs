using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Data;
using ArchivoDePasaportes.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArchivoDePasaportes.Controllers
{
    public class SourcesController : Controller
    {
        private ApplicationDbContext _context;
        public SourcesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult New()
        {
            return View("SourceForm");
        }

        public IActionResult Details(int id)
        {
            var sourceInDb = _context.Sources.SingleOrDefault(s => s.Id == id);
            if (sourceInDb == null)
                return NotFound();
            return View(sourceInDb);
        }


        public IActionResult Edit(int id)
        {
            var source = _context.Sources.SingleOrDefault(s => s.Id == id);
            if (source == null)
                return NotFound();
            return View("SourceForm", source);
        }

        [HttpPost]
        public IActionResult Save(Source source)
        {
            if (!ModelState.IsValid)
                return View("SourceForm", source);

            var sourceInDb = _context.Sources.SingleOrDefault(s => s.Id == source.Id);
            if(sourceInDb != null)
            {
                sourceInDb.Name = source.Name;
                sourceInDb.Description = source.Description;
            }
            else
                _context.Sources.Add(source);
            
            _context.SaveChanges();

            return RedirectToAction("Index", "People");
        }
    }
}