using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Clinic_Backend.Models;

namespace Clinic_Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<ClinicImage> ClinicImages { get; set; }
        public DbSet<ClinicVideo> ClinicVideos { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewImage> ReviewImages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingImage> BookingImages { get; set; }
        public DbSet<SiteContent> SiteContents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Clinic>()
               .Property(c => c.PriceRangeMin)
               .HasColumnType("decimal(18,2)");

            builder.Entity<Clinic>()
               .Property(c => c.PriceRangeMax)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Treatment>()
                .Property(t => t.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Review>()
                .Property(r => r.Price)
                .HasColumnType("decimal(3,2)");
        }
    }
}