﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Data;
using ArchivoDePasaportes.Dto;
using ArchivoDePasaportes.Models;
using ArchivoDePasaportes.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchivoDePasaportes.Controllers
{
    public class PeopleController : Controller
    {
        private readonly ApplicationDbContext _context; 
        public PeopleController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public IActionResult Index(string sortOrder, bool keepOrder, string searchString, int pageIndex)
        {
            ViewBag.ActualSortOrder = sortOrder;
            ViewBag.CISortParm = sortOrder == "ci" && !keepOrder ? "ci_desc" : "ci";
            ViewBag.NameSortParm = sortOrder == "name" && !keepOrder ? "name_desc" : "name";
            ViewBag.AddressSortParm = sortOrder == "address" && !keepOrder ? "address_desc" : "address";
            ViewBag.SearchString = searchString;
        
            var people = from p in _context.People select p;
        
            if (!String.IsNullOrEmpty(searchString))
                people = people.Where(p => 
                    p.LastName.Contains(searchString)||
                    p.FirstName.Contains(searchString)||
                    p.CI.Contains(searchString)||
                    p.Address.Contains(searchString));

            people = sortOrder switch
            {
                "ci_desc" => people.OrderByDescending(p => p.CI),
                "name" => people.OrderBy(p => p.LastName),
                "name_desc" => people.OrderByDescending(p => p.LastName),
                "address" => people.OrderBy(p => p.Address),
                "address_desc" => people.OrderByDescending(p => p.Address),
                _ => people.OrderBy(p => p.CI),
            };
            var pageSize = 5;
            int maxPageIndex = people.Count() % pageSize == 0 && people.Count() > 0 ? people.Count() / pageSize : people.Count() / pageSize + 1;
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageIndex = pageIndex > maxPageIndex ? maxPageIndex : pageIndex;
            ViewBag.PageIndex = pageIndex;
            ViewBag.MaxPageIndex = maxPageIndex;
            
            var people_list = people
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Source)
                .ToList();

            return View("ListPeople", people_list);
        }
        
        public IActionResult Details(long id)
        {
            var personInDb = _context.People.SingleOrDefault(p => p.Id == id);
            if (personInDb == null)
                return NotFound();
        
            personInDb.Source = _context.Sources.Single(s => s.Id == personInDb.SourceId);
            return View(personInDb);
        }

        [Authorize]
        public IActionResult Edit(long id)
        {
            var personInDb = _context.People.SingleOrDefault(p => p.Id == id);
            if (personInDb == null)
                return NotFound();

            var viewModel = new PersonFormViewModel()
            {
                PersonDto = new PersonDto(),
                OldCI = personInDb.CI,
                Sources = _context.Sources.ToList()
            };
            TransferData.Transfer(personInDb, viewModel.PersonDto, _context);
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

            var personInDb = _context.People.SingleOrDefault(p => p.CI == viewModel.OldCI);
            bool newPersonExists = _context.People.SingleOrDefault(p => p.CI == viewModel.PersonDto.CI) != null;
            if (personInDb != null)
            {
                bool isModifiedCI = viewModel.OldCI != viewModel.PersonDto.CI;
                if (isModifiedCI && newPersonExists)
                {
                    viewModel.ExistOtherInDb = true;
                    viewModel.Sources = _context.Sources.ToList();
                    return View("PersonForm", viewModel);
                }
                TransferData.Transfer(viewModel.PersonDto, personInDb, _context);
            }
            else if (newPersonExists)
            {
                viewModel.ExistOtherInDb = true;
                viewModel.Sources = _context.Sources.ToList();
                return View("PersonForm", viewModel);
            }
            else
            {
                var newPerson = new Person();
                TransferData.Transfer(viewModel.PersonDto, newPerson, _context);
                _context.People.Add(newPerson);
            }
            
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }   
}       