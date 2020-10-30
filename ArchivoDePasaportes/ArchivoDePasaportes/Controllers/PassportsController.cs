using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Areas.Identity.Data;
using ArchivoDePasaportes.Data;
using ArchivoDePasaportes.Dto;
using ArchivoDePasaportes.Extensions;
using ArchivoDePasaportes.Models;
using ArchivoDePasaportes.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchivoDePasaportes.Controllers
{
    [Authorize]
    public class PassportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PassportsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


        public IActionResult Index(string sortOrder, bool keepOrder, string searchString, int pageIndex)
        {
            ViewBag.ActualSortOrder = sortOrder;
            ViewBag.PassNoSortParm = sortOrder == "passNo" && !keepOrder ? "passNo_desc" : "passNo";
            ViewBag.OwnerCISortParm = sortOrder == "ownerCI" && !keepOrder ? "ownerCI_desc" : "ownerCI";
            ViewBag.OwnerNameSortParm = sortOrder == "ownerName" && !keepOrder ? "ownerName_desc" : "ownerName";
            ViewBag.ExpeditionSortParm = sortOrder == "expedition" && !keepOrder ? "expedition_desc" : "expedition";
            ViewBag.ExpirationSortParm = sortOrder == "expiration" && !keepOrder ? "expiration_desc" : "expiration";
            ViewBag.TypeSortParm = sortOrder == "type" && !keepOrder ? "type_desc" : "type";
            ViewBag.ArchivedSortParm = sortOrder == "archived" && !keepOrder ? "archived_desc" : "archived";
            ViewBag.SearchString = searchString;

            var passports = (from p in _context.Passports select p);


            if (!String.IsNullOrEmpty(searchString))
                passports = passports.Where(p =>
                    p.PassportNo.Contains(searchString) ||
                    p.Owner.CI.Contains(searchString) ||
                    (p.Owner.FirstName + " " + p.Owner.LastName).Contains(searchString)||
                    p.PassportType.Name.Contains(searchString));

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
                "type" => passports.OrderBy(p => p.PassportType),
                "type_desc" => passports.OrderByDescending(p => p.PassportType),
                "archived" => passports.OrderBy(p => p.IsPassportArchived),
                "archived_desc" => passports.OrderByDescending(p => p.IsPassportArchived),
                _ => passports.OrderBy(p => p.PassportNo),
            };
            var pageSize = Utils.PageSize;
            int maxPageIndex = passports.Count() % pageSize == 0 && passports.Count() > 0 ? passports.Count() / pageSize : passports.Count() / pageSize + 1;
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageIndex = pageIndex > maxPageIndex ? maxPageIndex : pageIndex;
            ViewBag.PageIndex = pageIndex;
            ViewBag.MaxPageIndex = maxPageIndex;

            var viewModel = new PassportFormViewModel();
            viewModel.PassportList = passports
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Owner)
                .Include(p => p.PassportType)
                .Include(p => p.Source)
                .ToList();
            viewModel.UserIsAdmin = Utils.IsCurrentUserAdmin(_context, _userManager, _httpContextAccessor);

            return View("ListPassports", viewModel);
        }
    
        public IActionResult Details(long id)
        {
            var passportInDb = _context.Passports.SingleOrDefault(p => p.Id == id);
            if (passportInDb == null)
                return NotFound();
            passportInDb.Owner = _context.People.Single(p => p.Id == passportInDb.OwnerId);
            passportInDb.Source = _context.Sources.Single(s => s.Id == passportInDb.SourceId);
            passportInDb.PassportType = _context.PassportTypes.Single(pt => pt.Id == passportInDb.PassportTypeId);
            
            var giveRecord = from gr in _context.GivePassports where gr.PassportId == passportInDb.Id orderby gr.Active select gr;

            var viewModel = new DetailsPassportViewModel()
            {
                GivePassports = giveRecord.ToList(),
                Passport = passportInDb
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            var viewModel = new PassportFormViewModel()
            {
                ValidDates = true,
                PassportTypes = _context.PassportTypes.ToList(),
                Sources = _context.Sources.ToList(),
            };
            return View("PassportForm", viewModel);
        }
        
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(long id)
        {
            var passportInDb = _context.Passports.SingleOrDefault(p => p.Id == id);
            if (passportInDb == null)
                return NotFound();

            var viewModel = new PassportFormViewModel()
            {
                ValidDates = true,
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

            viewModel.ValidDates = viewModel.PassportDto.ExpeditionDate <= viewModel.PassportDto.ExpirationDate;
            if (!viewModel.ValidDates)
                return View("PassportForm", viewModel);

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
                newPassport.IsPassportArchived = true;
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


            _context.OfficialTravels.RemoveRange((from ot in _context.OfficialTravels where ot.PassportId == passportInDb.Id select ot).ToList());
            _context.PermanentTravels.RemoveRange((from pt in _context.PermanentTravels where pt.PassportId == passportInDb.Id select pt).ToList());
            _context.Passports.Remove(passportInDb);
            _context.DroppedPassports.Add(viewModel.DroppedPassport);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult Give(long id)
        {
            var passportInDb = _context.Passports.SingleOrDefault(p => p.Id == id);
            if (passportInDb == null)
                return NotFound();
            if (!passportInDb.IsPassportArchived)
                return BadRequest();

            var viewModel = new GivePassportDto()
            {
                PassportNo = passportInDb.PassportNo,
                GiveDate = DateTime.Now
            };
            
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Give(GivePassportDto viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var passport = _context.Passports.SingleOrDefault(p => p.PassportNo == viewModel.PassportNo);
            if (passport == null || !passport.IsPassportArchived)
                return BadRequest();

            var givePassport = new GivePassport();
            TransferData.Transfer(viewModel, givePassport, _context);
            passport.IsPassportArchived = false;
            givePassport.Active = true;
            _context.GivePassports.Add(givePassport);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}