using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Data;
using ArchivoDePasaportes.Models;
using ArchivoDePasaportes.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchivoDePasaportes.Controllers
{
    public class PeopleController : Controller
    {
        private ApplicationDbContext _context; 
        public PeopleController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        
        public IActionResult Index(string sortOrder, string keepOrder, string searchString, int pageIndex)
        {
            if (String.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "id";
                keepOrder = "True";
            }
        
            ViewBag.IdSortParm = sortOrder == "id" && keepOrder != "True" ? "id_desc" : "id";
            ViewBag.NameSortParm = sortOrder == "name" && keepOrder != "True" ? "name_desc" : "name";
            ViewBag.AddressSortParm = sortOrder == "address" && keepOrder != "True" ? "address_desc" : "address";
            ViewBag.SearchString = searchString;
        
            var people = from p in _context.People select p;
        
            if (!String.IsNullOrEmpty(searchString))
                people = people.Where(p => 
                    p.LastName.Contains(searchString)||
                    p.FirstName.Contains(searchString)||
                    p.Id.Contains(searchString)||
                    p.Address.Contains(searchString));
        
            switch (sortOrder)
            {
                case "id":
                    people = people.OrderBy(p => p.Id);
                    break;
                case "id_desc":
                    people = people.OrderByDescending(p => p.Id);
                    break;
                case "name":
                    people = people.OrderBy(p => p.LastName);
                    break;
                case "name_desc":
                    people = people.OrderByDescending(p => p.LastName);
                    break;
                case "address":
                    people = people.OrderBy(p => p.Address);
                    break;
                case "address_desc":
                    people = people.OrderByDescending(p => p.Address);
                    break;
        
        
                default:
                    people = people.OrderBy(p => p.Id);
                    break;
            }
        
            var people_list = people.Include(p => p.Source).ToList();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageIndex = pageIndex > (people_list.Count / 5) ? (people_list.Count / 5) : pageIndex;
            ViewBag.PageIndex = pageIndex;
            ViewBag.MaxPageIndex = (int) people_list.Count / 5;
            int start_index = Math.Min(people_list.Count - 1, pageIndex * 5);
            int count = Math.Min(people_list.Count - start_index, 5);
            if (start_index != -1)
                people_list = people_list.GetRange(start_index, count);
        
            return View("ListPeople", people_list);
        }
        
        public IActionResult Details(string id)
        {
            var personInDb = _context.People.SingleOrDefault(p => p.Id == id);
            if (personInDb == null)
                return NotFound();
        
            personInDb.Source = _context.Sources.Single(s => s.Id == personInDb.SourceId);
            return View(personInDb);
        }
        
        public IActionResult Edit(string id)
        {
            var personInDb = _context.People.SingleOrDefault(p => p.Id == id);
            if (personInDb == null)
                return NotFound();
        
            var viewModel = new PersonFormViewModel()
            {
                Person = personInDb,
                OldId = personInDb.Id,
                Sources = _context.Sources.ToList()
            };
            return View("PersonForm", viewModel);
        }
        
        public IActionResult New()
        {
            var viewModel = new PersonFormViewModel()
            {
                Sources = _context.Sources.ToList()
            };
            return View("PersonForm", viewModel);
        }
        
        [HttpPost]
        public IActionResult Save(PersonFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Sources = _context.Sources.ToList();
                return View("PersonForm", viewModel);
            }
        
            if (viewModel.OldId == null || viewModel.OldId != viewModel.Person.Id)
            {
                var personInDb = _context.People.SingleOrDefault(p => p.Id == viewModel.Person.Id);
                if (personInDb != null)
                {
                    viewModel.ExistOtherInDb = true;
                    viewModel.Sources = _context.Sources.ToList();
                    return View("PersonForm", viewModel);
                }
            }
            if (viewModel.OldId == viewModel.Person.Id)
            {
                var personInDb = _context.People.Single(p => p.Id == viewModel.OldId);
                personInDb.FirstName = viewModel.Person.FirstName;
                personInDb.LastName = viewModel.Person.LastName;
                personInDb.BirthDay = viewModel.Person.BirthDay;
                personInDb.Address = viewModel.Person.Address;
            }
            else
            {
                _context.People.Add(viewModel.Person);
                if (viewModel.OldId != null)
                {
                    var oldPerson = _context.People.Single(p => p.Id == viewModel.OldId);
                    _context.People.Remove(oldPerson);
                }
            }
        
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }   
}       