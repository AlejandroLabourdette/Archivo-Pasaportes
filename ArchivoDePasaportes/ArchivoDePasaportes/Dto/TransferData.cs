using ArchivoDePasaportes.Data;
using ArchivoDePasaportes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Dto
{
    public static class TransferData
    {
        public static void Transfer(PassportDto origin, Passport destiny, ApplicationDbContext context)
        {
            destiny.PassportNo = origin.PassportNo;
            destiny.OwnerId = context.People.SingleOrDefault(per => per.CI == origin.OwnerCI).Id;
            destiny.PassportTypeId = origin.PassportTypeId;
            destiny.SourceId = origin.SourceId;
            destiny.ExpeditionDate = origin.ExpeditionDate;
            destiny.ExpirationDate = origin.ExpirationDate;
            destiny.IsPassportArchived = origin.IsPassportArchived;
        }
        public static void Transfer(Passport origin, PassportDto destiny, ApplicationDbContext context)
        {
            destiny.PassportNo = origin.PassportNo;
            destiny.OwnerCI = context.People.SingleOrDefault(per => per.Id == origin.OwnerId).CI;
            destiny.PassportTypeId = origin.PassportTypeId;
            destiny.SourceId = origin.SourceId;
            destiny.ExpeditionDate = origin.ExpeditionDate;
            destiny.ExpirationDate = origin.ExpirationDate;
            destiny.IsPassportArchived = origin.IsPassportArchived;
        }


        public static void Transfer(PersonDto origin, Person destiny, ApplicationDbContext context)
        {
            destiny.CI = origin.CI;
            destiny.FirstName = origin.FirstName;
            destiny.LastName = origin.LastName;
            destiny.Address = origin.Address;
            destiny.BirthDay = origin.BirthDay;
            destiny.Ocupation = origin.Ocupation;
            destiny.SourceId = origin.SourceId;
        }
        public static void Transfer(Person origin, PersonDto destiny, ApplicationDbContext context)
        {
            destiny.CI = origin.CI;
            destiny.FirstName = origin.FirstName;
            destiny.LastName = origin.LastName;
            destiny.Address = origin.Address;
            destiny.BirthDay = origin.BirthDay;
            destiny.Ocupation = origin.Ocupation;
            destiny.SourceId = origin.SourceId;
        }


        public static void Transfer(DroppedPassportDto origin, DroppedPassport destiny, ApplicationDbContext context)
        {
            destiny.PassportNo = origin.PassportNo;
            destiny.OwnerId = context.People.SingleOrDefault(per => per.CI == origin.OwnerCI).Id;
            destiny.PassportTypeId = origin.PassportTypeId;
            destiny.SourceId = origin.SourceId;
            destiny.ExpeditionDate = origin.ExpeditionDate;
            destiny.ExpirationDate = origin.ExpirationDate;
            destiny.DropCauseId = origin.DropCauseId;
            destiny.Details = origin.Details;

        }
        public static void Transfer(DroppedPassport origin, DroppedPassportDto destiny, ApplicationDbContext context)
        {
            destiny.PassportNo = origin.PassportNo;
            destiny.OwnerCI = context.People.SingleOrDefault(per => per.Id == origin.OwnerId).CI;
            destiny.PassportTypeId = origin.PassportTypeId;
            destiny.SourceId = origin.SourceId;
            destiny.ExpeditionDate = origin.ExpeditionDate;
            destiny.ExpirationDate = origin.ExpirationDate;
            destiny.DropCauseId = origin.DropCauseId;
            destiny.Details = origin.Details;

        }
    

        public static void Transfer(GivePassportDto origin, GivePassport destiny, ApplicationDbContext context)
        {
            destiny.PassportId = context.Passports.SingleOrDefault(p => p.PassportNo == origin.PassportNo).Id;
            destiny.GiveDate = origin.GiveDate;
            destiny.ExpectedReturn = origin.ExpectedReturn;
            destiny.Description = origin.Description;
        }
        public static void Transfer(GivePassport origin, GivePassportDto destiny, ApplicationDbContext context)
        {
            destiny.PassportNo = context.Passports.SingleOrDefault(p => p.Id == origin.PassportId).PassportNo;
            destiny.GiveDate = origin.GiveDate;
            destiny.ExpectedReturn = origin.ExpectedReturn;
            destiny.Description = origin.Description;
        }
        public static void Transfer(List<GivePassport> origin, List<GivePassportDto> destiny, ApplicationDbContext context)
        {
            foreach (var givePass in origin)
            {
                var dto = new GivePassportDto();
                Transfer(givePass, dto, context);
                destiny.Add(dto);
            }
        }
    
    
        public static void Transfer(PassInfoOfficialTravelDto origin, OfficialTravel destiny, ApplicationDbContext context)
        {
            destiny.PassportId = context.Passports.Single(p => p.PassportNo == origin.PassportNo).Id;
            destiny.OccupationId = origin.OcupationId;
            destiny.ReturnDate = origin.ReturnDate;
        }
        public static void Transfer (OfficialTravel origin, PassInfoOfficialTravelDto destiny, ApplicationDbContext context)
        {
            destiny.PassportNo = context.Passports.Single(p => p.Id == origin.PassportId).PassportNo;
            destiny.OcupationId = origin.OccupationId;
            destiny.ReturnDate = origin.ReturnDate;
        }


        public static void Transfer(PassInfoPermanentTravelDto origin, PermanentTravel destiny, ApplicationDbContext context)
        {
            destiny.PassportId = context.Passports.Single(p => p.PassportNo == origin.PassportNo).Id;
            destiny.OccupationId = origin.OcupationId;
        }
        public static void Transfer(PermanentTravel origin, PassInfoPermanentTravelDto destiny, ApplicationDbContext context)
        {
            destiny.PassportNo = context.Passports.Single(p => p.Id == origin.PassportId).PassportNo;
            destiny.OcupationId = origin.OccupationId;
        }
    }
}
