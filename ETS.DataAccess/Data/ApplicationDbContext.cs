using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ETS.Models.Models;
using ETS.Models.Models.Authentication;

namespace ETS.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
   

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
      
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        
        public DbSet<Duty> duties { get; set; }
        public DbSet<Registration> registrations { get; set; }


    }
}
