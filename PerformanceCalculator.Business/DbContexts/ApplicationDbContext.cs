using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PerformanceCalculator.Common.Models;
using PerformanceCalculator.Common.Models.Audit;

namespace PerformanceCalculator.Business.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Exam> Exams { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<TeacherAudit> TeacherAudits { get; set; }
        public DbSet<CourseAudit> CourseAudits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Exam>()
                .Property(e => e.ObtainedMark)
                .HasDefaultValue(0.00);

            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Exams)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Student)
                .WithMany(s => s.Courses)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return (await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken));
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            var utcNow = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                // for entities that inherit from BaseModel,
                // set UpdatedOn / CreatedOn appropriately
                if (entry.Entity is BaseModel trackable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            // set the updated date to "now"
                            trackable.UpdatedAt = utcNow;

                            // mark property as "don't touch"
                            // we don't want to update on a Modify operation
                            entry.Property("CreatedAt").IsModified = false;
                            break;

                        case EntityState.Added:
                            // set both updated and created date to "now"
                            trackable.CreatedAt = utcNow;
                            trackable.UpdatedAt = utcNow;
                            break;
                        case EntityState.Detached:
                            break;
                        case EntityState.Unchanged:
                            break;
                        case EntityState.Deleted:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}