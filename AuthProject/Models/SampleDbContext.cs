using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AuthProject.Models
{
    public partial class SampleDbContext : DbContext
    {
        public SampleDbContext()
        {
        }

        public SampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<UserTable> UserTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=SampleDb;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTable>(entity =>
            {
                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.FName)
                    .HasColumnName("fName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");

                entity.Property(e => e.LName)
                    .HasColumnName("lName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .IsUnicode(false);
            });
        }
    }
}
