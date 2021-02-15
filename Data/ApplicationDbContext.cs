using CoreStoreMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoreStoreMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductsType> ProductsTypes { get; set; }
        public DbSet<SpecialTag> SpecialTags { get; set; }
        // Урок 4
        public DbSet<Product> Products { get; set; }

        // Урок 10
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<ProductsForAppointment> ProductsForAppointments { get; set; }
    }
}
