using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MRS.Infrastructure.Data.Identity;

namespace MRS.Infrastructure.Data
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>,int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            // this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
             this.Database.SetCommandTimeout(12000000);
            //this.Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserMovie>()
                .HasOne(um => um.Movie)
                .WithMany(m => m.UserMovies)
                .HasForeignKey(um => um.MovieId);

            builder.Entity<UserMovie>()
                .HasOne(um => um.User)
                .WithMany(u => u.UserMovies)
                .HasForeignKey(um => um.UserId);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.UserMovies)
                .WithOne(um => um.User)
                .HasForeignKey(um => um.UserId);



            builder.Entity<UserMovie>()
                .HasKey(um => new { um.UserId, um.MovieId });
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<UserMovie> UserMovies { get; set; }

    }
}