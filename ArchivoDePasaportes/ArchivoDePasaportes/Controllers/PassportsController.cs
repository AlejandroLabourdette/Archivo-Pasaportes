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
    public class PassportsController : Controller
    {
        private ApplicationDbContext _context;
        public PassportsController(ApplicationDbContext context)
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

            ViewBag.ActualSortOrder = sortOrder;
            ViewBag.IdSortParm = sortOrder == "id" && keepOrder != "True" ? "id_desc" : "id";
            ViewBag.OwnerIdSortParm = sortOrder == "ownerId" && keepOrder != "True" ? "ownerId_desc" : "ownerId";
            ViewBag.OwnerNameSortParm = sortOrder == "ownerName" && keepOrder != "True" ? "ownerName_desc" : "ownerName";
            ViewBag.ExpeditionSortParm = sortOrder == "expedition" && keepOrder != "True" ? "expedition_desc" : "expedition";
            ViewBag.ExpirationSortParm = sortOrder == "expiration" && keepOrder != "True" ? "expiration_desc" : "expiration";
            ViewBag.SearchString = searchString;

            var passports = (from p in _context.Passports select p);


            if (!String.IsNullOrEmpty(searchString))
                passports = passports.Where(p =>
                    p.Id.Contains(searchString)||
                    (p.Owner.FirstName + " " + p.Owner.LastName).Contains(searchString)||
                    p.OwnerId.Contains(searchString));           
            
            switch (sortOrder)
            {
                case "id":
                    passports = passports.OrderBy(p => p.Id);
                    break;
                case "id_desc":
                    passports = passports.OrderByDescending(p => p.Id);
                    break;
                case "ownerId":
                    passports = passports.OrderBy(p => p.OwnerId);
                    break;
                case "ownerId_desc":
                    passports = passports.OrderByDescending(p => p.OwnerId);
                    break;
                case "ownerName":
                    passports = passports.OrderBy(p => p.Owner.FullName());
                    break;
                case "ownerName_desc":
                    passports = passports.OrderByDescending(p => p.Owner.FullName());
                    break;
                case "expedition":
                    passports = passports.OrderBy(p => p.ExpeditionDate);
                    break;
                case "expedition_desc":
                    passports = passports.OrderByDescending(p => p.ExpeditionDate);
                    break;
                case "expiration":
                    passports = passports.OrderBy(p => p.ExpirationDate);
                    break;
                case "expiration_desc":
                    passports = passports.OrderByDescending(p => p.ExpirationDate);
                    break;

                default:
                    passports = passports.OrderBy(p => p.Id);
                    break;
            }

            var passports_list = passports
                .Include(p => p.Owner)
                .Include(p=>p.PassportType)
                .Include(p=>p.Source)
                .ToList();
            
            var pageSize = 5;
            int maxPageIndex = passports_list.Count / pageSize + 1;
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageIndex = pageIndex > maxPageIndex ? maxPageIndex : pageIndex;
            ViewBag.PageIndex = pageIndex;
            ViewBag.MaxPageIndex = maxPageIndex;

            passports_list = passports_list
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View("ListPassports", passports_list);
        }
    
        public IActionResult Details(string id)
        {
            var passportInDb = _context.Passports.SingleOrDefault(p => p.Id == id);
            if (passportInDb == null)
                return NotFound();

            passportInDb.Owner = _context.People.Single(p => p.Id == passportInDb.OwnerId);
            passportInDb.Source = _context.Sources.Single(s => s.Id == passportInDb.SourceId);
            passportInDb.PassportType = _context.PassportTypes.Single(pt => pt.Id == passportInDb.PassportTypeId);
            return View(passportInDb);
        }

        public IActionResult New()
        {
            var viewModel = new PassportFormViewModel()
            {
                PassportTypes = _context.PassportTypes.ToList(),
                Sources = _context.Sources.ToList()
            };
            return View("PassportForm", viewModel);
        }

        public IActionResult Edit(string id)
        {
            var passportInDb = _context.Passports.SingleOrDefault(p => p.Id == id);
            if (passportInDb == null)
                return NotFound();

            var viewModel = new PassportFormViewModel()
            {
                Passport = passportInDb,
                OldId = passportInDb.Id,
                PassportTypes = _context.PassportTypes.ToList(),
                Sources = _context.Sources.ToList()
            };
            return View("PassportForm", viewModel);
        }

        public IActionResult Save(PassportFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Sources = _context.Sources.ToList();
                viewModel.PassportTypes = _context.PassportTypes.ToList();
                return View("PassportForm", viewModel);
            }

            if (viewModel.OldId == null || viewModel.OldId != viewModel.Passport.Id)
            {
                var passportInDb = _context.Passports.SingleOrDefault(p => p.Id == viewModel.Passport.Id);
                var droppedPassportInDb = _context.DroppedPassports.SingleOrDefault(p => p.Id == viewModel.Passport.Id);
                if (passportInDb != null || droppedPassportInDb != null)
                {
                    viewModel.ExistOtherInDb = true;
                    viewModel.Sources = _context.Sources.ToList();
                    viewModel.PassportTypes = _context.PassportTypes.ToList();
                    return View("PassportForm", viewModel);
                }

                var personInDb = _context.People.SingleOrDefault(p => p.Id == viewModel.Passport.OwnerId);
                if (personInDb == null)
                {
                    viewModel.NotExistThisPersonInDb = true; ;
                    viewModel.Sources = _context.Sources.ToList();
                    viewModel.PassportTypes = _context.PassportTypes.ToList();
                    return View("PassportForm", viewModel);
                }
            }

            if (viewModel.OldId == viewModel.Passport.Id)
            {
                var passportInDb = _context.Passports.Single(p => p.Id == viewModel.OldId);

                passportInDb.OwnerId = viewModel.Passport.OwnerId;
                passportInDb.PassportTypeId = viewModel.Passport.PassportTypeId;
                passportInDb.SourceId = viewModel.Passport.SourceId;
                passportInDb.ExpeditionDate = viewModel.Passport.ExpeditionDate;
                passportInDb.ExpirationDate = viewModel.Passport.ExpirationDate;
            }
            else
            {
                _context.Passports.Add(viewModel.Passport);
                if (viewModel.OldId != null)
                {
                    var oldPassport = _context.Passports.Single(p => p.Id == viewModel.OldId);
                    _context.Passports.Remove(oldPassport);
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Drop(string id)
        {
            var passportInDb = _context.Passports.SingleOrDefault(p => p.Id == id);
            if (passportInDb == null)
                return NotFound();
            var viewModel = new DropPassportViewModel()
            {
                DroppedPassport = new DroppedPassport()
                {
                    Id = passportInDb.Id,
                    OwnerId = passportInDb.OwnerId,
                    PassportTypeId = passportInDb.PassportTypeId,
                    SourceId = passportInDb.SourceId,
                    ExpeditionDate = passportInDb.ExpeditionDate,
                    ExpirationDate = passportInDb.ExpirationDate
                },
                DropCauses = _context.DropCauses.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Drop(DropPassportViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.DropCauses = _context.DropCauses.ToList();
                return View(viewModel);
            }

            var passportInDb = _context.Passports.SingleOrDefault(p => p.Id == viewModel.DroppedPassport.Id);
            if (passportInDb == null)
                return BadRequest();

            var droppedPassportInDb = _context.DroppedPassports.SingleOrDefault(dp => dp.Id == viewModel.DroppedPassport.Id);
            if (droppedPassportInDb != null)
                return BadRequest();

            _context.Passports.Remove(passportInDb);
            _context.DroppedPassports.Add(viewModel.DroppedPassport);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}