using DebugMeBackend.Entities;
using Microsoft.EntityFrameworkCore;

namespace DebugMeBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Emotion> Emotions { get; set; } = null!;
        public DbSet<EventLog> EventLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.CreatedAt)
                      .IsRequired();

                entity.HasIndex(u => u.Email)
                      .IsUnique();
            });

            modelBuilder.Entity<Emotion>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Description)
                      .HasMaxLength(250);
            });

            modelBuilder.Entity<EventLog>(entity =>
            {
                entity.HasKey(el => el.Id);

                entity.Property(el => el.Description)
                      .HasMaxLength(500);

                entity.Property(el => el.Intensity)
                      .IsRequired();

                entity.Property(el => el.EventDate)
                      .IsRequired();

                entity.HasOne(el => el.User)
                      .WithMany()
                      .HasForeignKey(el => el.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(el => el.Emotion)
                      .WithMany()
                      .HasForeignKey(el => el.EmotionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}