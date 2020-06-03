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
            
            var passports_list = passports
                .Include(p => p.Owner)
                .Include(p=>p.PassportType)
                .Include(p=>p.Source)
                .ToList();

            switch (sortOrder)
            {
                case "id":
                    passports_list = passports_list.OrderBy(p => p.Id).ToList();
                    break;
                case "id_desc":
                    passports_list = passports_list.OrderByDescending(p => p.Id).ToList();
                    break;
                case "ownerId":
                    passports_list = passports_list.OrderBy(p => p.OwnerId).ToList();
                    break;
                case "ownerId_desc":
                    passports_list = passports_list.OrderByDescending(p => p.OwnerId).ToList();
                    break;
                case "ownerName":
                    passports_list = passports_list.OrderBy(p => p.Owner.FullName()).ToList();
                    break;
                case "ownerName_desc":
                    passports_list = passports_list.OrderByDescending(p => p.Owner.FullName()).ToList();
                    break;
                case "expedition":
                    passports_list = passports_list.OrderBy(p => p.ExpeditionDate).ToList();
                    break;
                case "expedition_desc":
                    passports_list = passports_list.OrderByDescending(p => p.ExpeditionDate).ToList();
                    break;
                case "expiration":
                    passports_list = passports_list.OrderBy(p => p.ExpirationDate).ToList();
                    break;
                case "expiration_desc":
                    passports_list = passports_list.OrderByDescending(p => p.ExpirationDate).ToList();
                    break;

                default:
                    passports_list = passports_list.OrderBy(p => p.Id).ToList();
                    break;
            }


            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageIndex = pageIndex > (passports_list.Count / 5) ? (passports_list.Count / 5) : pageIndex;
            ViewBag.PageIndex = pageIndex;
            ViewBag.MaxPageIndex = (int)passports_list.Count / 5;
            int start_index = Math.Min(passports_list.Count - 1, pageIndex * 5);
            int count = Math.Min(passports_list.Count - start_index, 5);
            if (start_index != -1)
                passports_list = passports_list.GetRange(start_index, count);

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
                if (passportInDb != null)
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