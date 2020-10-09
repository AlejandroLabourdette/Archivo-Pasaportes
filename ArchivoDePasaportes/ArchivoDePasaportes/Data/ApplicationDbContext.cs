using System;
using System.Collections.Generic;
using System.Text;
using ArchivoDePasaportes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ArchivoDePasaportes.Areas.Identity.Data;

namespace ArchivoDePasaportes.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Source> Sources { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<PassportType> PassportTypes { get; set; }
        public DbSet<Passport> Passports { get; set; }
        public DbSet<DropCause> DropCauses { get; set; }
        public DbSet<DroppedPassport> DroppedPassports { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Occupation> Occupations { get; set; }
        public DbSet<OfficialTravel> OfficialTravels { get; set; }
        public DbSet<PermanentTravel> PermanentTravels { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
