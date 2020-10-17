using System;
using ArchivoDePasaportes.Areas.Identity.Data;
using ArchivoDePasaportes.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ArchivoDePasaportes.Areas.Identity.IdentityHostingStartup))]
namespace ArchivoDePasaportes.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}