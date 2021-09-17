using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Models
{
    public class UsedContext : DbContext
    {
        public UsedContext(DbContextOptions<UsedContext> options) : base(options)
        {
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                        .Property(p => p.Username)
                        .IsRequired();
            
            modelBuilder.Entity<Users>()
                        .Property(p => p.Password)
                        .IsRequired();

            modelBuilder.Entity<Student>()
                        .Property(p => p.FirstName)
                        .IsRequired()
                        .HasMaxLength(100);
            
            modelBuilder.Entity<Student>()
                        .Property(p => p.LastName)
                        .IsRequired()
                        .HasMaxLength(100);

            modelBuilder.Entity<Student>()
                        .HasOne(p => p.Grade)
                        .WithMany(g => g.Students)
                        .HasForeignKey(p => p.GradeId);

             modelBuilder.Entity<Student>()
                         .HasOne(p => p.Teacher)
                         .WithMany()
                         .HasForeignKey(p => p.TeacherId);

            modelBuilder.Entity<Teacher>()
                        .Property(p => p.FirstName)
                        .IsRequired()
                        .HasMaxLength(100);
            
            modelBuilder.Entity<Teacher>()
                        .Property(p => p.LastName)
                        .IsRequired()
                        .HasMaxLength(100);
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<API.Models.Grade> Grade { get; set; }
    }
}