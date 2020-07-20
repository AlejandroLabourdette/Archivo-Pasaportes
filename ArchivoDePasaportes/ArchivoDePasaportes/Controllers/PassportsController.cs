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

        public IActionResult Index(string sortOrder, bool keepOrder, string searchString, int pageIndex)
        {
            ViewBag.ActualSortOrder = sortOrder;
            ViewBag.IdSortParm = sortOrder == "id" && !keepOrder ? "id_desc" : "id";
            ViewBag.OwnerIdSortParm = sortOrder == "ownerId" && !keepOrder ? "ownerId_desc" : "ownerId";
            ViewBag.OwnerNameSortParm = sortOrder == "ownerName" && !keepOrder ? "ownerName_desc" : "ownerName";
            ViewBag.ExpeditionSortParm = sortOrder == "expedition" && !keepOrder ? "expedition_desc" : "expedition";
            ViewBag.ExpirationSortParm = sortOrder == "expiration" && !keepOrder ? "expiration_desc" : "expiration";
            ViewBag.SearchString = searchString;

            var passports = (from p in _context.Passports select p);


            if (!String.IsNullOrEmpty(searchString))
                passports = passports.Where(p =>
                    p.PassportNo.Contains(searchString) ||
                    p.OwnerId.Contains(searchString) ||
                    _context.People.Single(per => per.Id == p.OwnerId).FullName().Contains(searchString));
            
            switch (sortOrder)
            {
                case "id_desc":
                    passports = passports.OrderByDescending(p => p.PassportNo);
                    break;
                case "ownerId":
                    passports = passports.OrderBy(p => p.OwnerId);
                    break;
                case "ownerId_desc":
                    passports = passports.OrderByDescending(p => p.OwnerId);
                    break;
                case "ownerName":
                    passports = passports.OrderBy(p => p.Owner.NameWithComa());
                    break;
                case "ownerName_desc":
                    passports = passports.OrderByDescending(p => p.Owner.NameWithComa());
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
                    passports = passports.OrderBy(p => p.PassportNo);
                    break;
            }
            
            var pageSize = 5;
            int maxPageIndex = passports.Count() % pageSize == 0 && passports.Count() > 0 ? passports.Count() / pageSize : passports.Count() / pageSize + 1;
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageIndex = pageIndex > maxPageIndex ? maxPageIndex : pageIndex;
            ViewBag.PageIndex = pageIndex;
            ViewBag.MaxPageIndex = maxPageIndex;

            var passports_list = passports
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Owner)
                .Include(p => p.PassportType)
                .Include(p => p.Source)
                .ToList();

            return View("ListPassports", passports_list);
        }
    
        public IActionResult Details(long id)
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

        public IActionResult Edit(long id)
        {
            var passportInDb = _context.Passports.SingleOrDefault(p => p.Id == id);
            if (passportInDb == null)
                return NotFound();

            var viewModel = new PassportFormViewModel()
            {
                Passport = passportInDb,
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

            var personInDb = _context.People.SingleOrDefault(p => p.Id == viewModel.Passport.OwnerId);
            if (personInDb == null)
            {
                viewModel.NotExistThisPersonInDb = true;
                viewModel.Sources = _context.Sources.ToList();
                viewModel.PassportTypes = _context.PassportTypes.ToList();
                return View("PassportForm", viewModel);
            }

            var passportInDb = _context.Passports.SingleOrDefault(p => p.Id == viewModel.Passport.Id);
            var droppedPassportInDb = _context.DroppedPassports.SingleOrDefault(p => p.Id == viewModel.Passport.Id);
            if (passportInDb!=null || droppedPassportInDb!=null)
            {
                bool passportExists = _context.Passports.SingleOrDefault(p => p.PassportNo == viewModel.Passport.PassportNo) != null ||
                              _context.DroppedPassports.SingleOrDefault(p => p.PassportNo == viewModel.Passport.PassportNo) != null;
                bool isModifiedPassNoInPassDB = passportInDb.PassportNo != viewModel.Passport.PassportNo;
                bool isModifiedPassNoInDropPassDB = droppedPassportInDb.PassportNo != viewModel.Passport.PassportNo;
                if ((isModifiedPassNoInPassDB || isModifiedPassNoInDropPassDB) && passportExists)
                {
                    viewModel.ExistOtherInDb = true;
                    viewModel.Sources = _context.Sources.ToList();
                    viewModel.PassportTypes = _context.PassportTypes.ToList();
                    return View("PassportForm", viewModel);
                }

                if (passportInDb != null)
                {        
                    passportInDb.PassportNo = viewModel.Passport.PassportNo;
                    passportInDb.OwnerId = viewModel.Passport.OwnerId;
                    passportInDb.PassportTypeId = viewModel.Passport.PassportTypeId;
                    passportInDb.SourceId = viewModel.Passport.SourceId;
                    passportInDb.ExpeditionDate = viewModel.Passport.ExpeditionDate;
                    passportInDb.ExpirationDate = viewModel.Passport.ExpirationDate;
                }
                else
                {
                    droppedPassportInDb.PassportNo = viewModel.Passport.PassportNo;
                    droppedPassportInDb.OwnerId = viewModel.Passport.OwnerId;
                    droppedPassportInDb.PassportTypeId = viewModel.Passport.PassportTypeId;
                    droppedPassportInDb.SourceId = viewModel.Passport.SourceId;
                    droppedPassportInDb.ExpeditionDate = viewModel.Passport.ExpeditionDate;
                    droppedPassportInDb.ExpirationDate = viewModel.Passport.ExpirationDate;
                }
            }

            else
                _context.Passports.Add(viewModel.Passport);

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Drop(long id)
        {
            var passportInDb = _context.Passports.SingleOrDefault(p => p.Id == id);
            if (passportInDb == null)
                return NotFound();
            var viewModel = new DropPassportViewModel()
            {
                DroppedPassport = new DroppedPassport()
                {
                    PassportNo = passportInDb.PassportNo,
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

            var passportInDb = _context.Passports.SingleOrDefault(p => p.PassportNo == viewModel.DroppedPassport.PassportNo);
            if (passportInDb == null)
                return BadRequest();

            var droppedPassportInDb = _context.DroppedPassports.SingleOrDefault(dp => dp.PassportNo == viewModel.DroppedPassport.PassportNo);
            if (droppedPassportInDb != null)
                return BadRequest();

            _context.Passports.Remove(passportInDb);
            _context.DroppedPassports.Add(viewModel.DroppedPassport);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}