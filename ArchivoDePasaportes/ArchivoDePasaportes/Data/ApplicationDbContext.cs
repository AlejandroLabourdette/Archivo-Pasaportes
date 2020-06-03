using System;
using System.Collections.Generic;
using System.Text;
using ArchivoDePasaportes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArchivoDePasaportes.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Source> Sources { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<PassportType> PassportTypes { get; set; }
        public DbSet<Passport> Passports { get; set; }
        public DbSet<DropCause> DropCauses { get; set; }
        public DbSet<DroppedPassport> DroppedPassports { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
