using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HOB_WebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HOB_WebApp.Data
{
    public class HOB_WebAppContext : IdentityDbContext<User>
    {
        public HOB_WebAppContext (DbContextOptions<HOB_WebAppContext> options)
            : base(options)
        {
        }

        public DbSet<HOB_WebApp.Models.ContentModel> ContentModel { get; set; }

        public DbSet<HOB_WebApp.Models.ServiceProviderModel> ServiceProviderModel { get; set; }

        public DbSet<HOB_WebApp.Models.MobileUsers> MobileUsers { get; set; }

        public DbSet<HOB_WebApp.Models.HomeCodes> HomeCodes { get; set; }
    }
}
