using FitnessAppBackend.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessAppBackend.Data.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<HealthAdvice> HealthAdvice { get; set; }
        public DbSet<ExerciseLog> ExerciseLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<WorkoutPlan>()
                .HasOne(w => w.User)
                .WithMany(u => u.WorkoutPlans)
                .HasForeignKey(w => w.UserId);

            // Add indexes
            builder.Entity<WorkoutPlan>()
                .HasIndex(w => w.UserId);

            builder.Entity<HealthAdvice>()
                .HasIndex(h => h.Category);

            builder.Entity<ExerciseLog>()
                .HasOne(e => e.User);
        }
    }
}