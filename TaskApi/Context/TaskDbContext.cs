using Microsoft.EntityFrameworkCore;
using TaskApi.Models;

namespace TaskApi.Context
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
        {
        }

        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<TaskKeyword> TaskKeywords { get; set; }
        public DbSet<TaskPerformanceRating> TaskPerformanceRatings { get; set; }
        public DbSet<SimilarTaskReference> SimilarTaskReferences { get; set; }
        public DbSet<TaskActivity> TaskActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Task Configuration
            modelBuilder.Entity<Models.Task>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.TaskName).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Description).HasMaxLength(1000);
                entity.Property(t => t.CreatedAt).IsRequired();
                entity.Property(t => t.Status).IsRequired();
                entity.Property(t => t.Priority).IsRequired();

                entity.HasMany(t => t.Keywords)
                      .WithOne(k => k.Task)
                      .HasForeignKey(k => k.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(t => t.PerformanceRatings)
                      .WithOne(p => p.Task)
                      .HasForeignKey(p => p.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TaskKeyword Configuration
            modelBuilder.Entity<TaskKeyword>(entity =>
            {
                entity.HasKey(k => k.Id);
                entity.Property(k => k.Keyword).IsRequired().HasMaxLength(100);
                entity.Property(k => k.Weight).HasDefaultValue(1.0);

                entity.HasIndex(k => k.Keyword);
            });

            // TaskPerformanceRating Configuration
            modelBuilder.Entity<TaskPerformanceRating>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.PerformanceScore).IsRequired();
                entity.Property(p => p.SimilarityScore).IsRequired();
                entity.Property(p => p.TimeEfficiencyScore).IsRequired();
                entity.Property(p => p.CalculatedAt).IsRequired();

                entity.HasMany(p => p.SimilarTasks)
                      .WithOne(s => s.PerformanceRating)
                      .HasForeignKey(s => s.PerformanceRatingId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // SimilarTaskReference Configuration
            modelBuilder.Entity<SimilarTaskReference>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.SimilarityPercentage).IsRequired();
                entity.Property(s => s.SimilarTaskDurationMinutes).IsRequired();
            });

            // TaskActivity Configuration
            modelBuilder.Entity<TaskActivity>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.ActivityType).IsRequired().HasMaxLength(50);
                entity.Property(a => a.ActivityTime).IsRequired();
                entity.Property(a => a.Comments).HasMaxLength(500);

                entity.HasOne(a => a.Task)
                      .WithMany()
                      .HasForeignKey(a => a.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
