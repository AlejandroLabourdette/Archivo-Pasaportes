using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Data;
using ArchivoDePasaportes.Dto;
using ArchivoDePasaportes.Models;
using ArchivoDePasaportes.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchivoDePasaportes.Controllers
{
    public class PassportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PassportsController(ApplicationDbContext context)
        {
            _context = context;
        }       


        public IActionResult Index(string sortOrder, bool keepOrder, string searchString, int pageIndex)
        {
            ViewBag.ActualSortOrder = sortOrder;
            ViewBag.PassNoSortParm = sortOrder == "passNo" && !keepOrder ? "passNo_desc" : "passNo";
            ViewBag.OwnerCISortParm = sortOrder == "ownerCI" && !keepOrder ? "ownerCI_desc" : "ownerCI";
            ViewBag.OwnerNameSortParm = sortOrder == "ownerName" && !keepOrder ? "ownerName_desc" : "ownerName";
            ViewBag.ExpeditionSortParm = sortOrder == "expedition" && !keepOrder ? "expedition_desc" : "expedition";
            ViewBag.ExpirationSortParm = sortOrder == "expiration" && !keepOrder ? "expiration_desc" : "expiration";
            ViewBag.SearchString = searchString;

            var passports = (from p in _context.Passports select p);


            if (!String.IsNullOrEmpty(searchString))
                passports = passports.Where(p =>
                    p.PassportNo.Contains(searchString) ||
                    p.Owner.CI.Contains(searchString) ||
                    (p.Owner.FirstName + " " + p.Owner.LastName).Contains(searchString));

            passports = sortOrder switch
            {
                "passNo_desc" => passports.OrderByDescending(p => p.PassportNo),
                "ownerCI" => passports.OrderBy(p => p.Owner.CI),
                "ownerCI_desc" => passports.OrderByDescending(p => p.Owner.CI),
                "ownerName" => passports.OrderBy(p => p.Owner.LastName + ", " + p.Owner.FirstName),
                "ownerName_desc" => passports.OrderByDescending(p => p.Owner.LastName + ", " + p.Owner.FirstName),
                "expedition" => passports.OrderBy(p => p.ExpeditionDate),
                "expedition_desc" => passports.OrderByDescending(p => p.ExpeditionDate),
                "expiration" => passports.OrderBy(p => p.ExpirationDate),
                "expiration_desc" => passports.OrderByDescending(p => p.ExpirationDate),
                _ => passports.OrderBy(p => p.PassportNo),
            };
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
                PassportTypes = _context.PassportTypes.ToList(),
                Sources = _context.Sources.ToList(),
                OldPassportNo = passportInDb.PassportNo,
                PassportDto = new PassportDto()
            };
            TransferData.Transfer(passportInDb, viewModel.PassportDto, _context);
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

            var personInDb = _context.People.SingleOrDefault(p => p.CI == viewModel.PassportDto.OwnerCI);
            if (personInDb == null)
            {
                viewModel.NotExistThisPersonInDb = true;
                viewModel.Sources = _context.Sources.ToList();
                viewModel.PassportTypes = _context.PassportTypes.ToList();
                return View("PassportForm", viewModel);
            }

            var passportInDb = _context.Passports.SingleOrDefault(p => p.PassportNo == viewModel.OldPassportNo);
            bool newPassportExists = _context.Passports.SingleOrDefault(p => p.PassportNo == viewModel.PassportDto.PassportNo) != null;
            if (passportInDb != null)
            {
                bool isModifiedPassNo = viewModel.OldPassportNo != viewModel.PassportDto.PassportNo;
                if (isModifiedPassNo && newPassportExists)
                {
                    viewModel.ExistOtherInDb = true;
                    viewModel.Sources = _context.Sources.ToList();
                    viewModel.PassportTypes = _context.PassportTypes.ToList();
                    return View("PassportForm", viewModel);
                }
                TransferData.Transfer(viewModel.PassportDto, passportInDb, _context);
            }
            else if (newPassportExists)
            {
                viewModel.ExistOtherInDb = true;
                viewModel.Sources = _context.Sources.ToList();
                viewModel.PassportTypes = _context.PassportTypes.ToList();
                return View("PassportForm", viewModel);
            }
            else
            {
                var newPassport = new Passport();
                TransferData.Transfer(viewModel.PassportDto, newPassport, _context);
                _context.Passports.Add(newPassport);
            }

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