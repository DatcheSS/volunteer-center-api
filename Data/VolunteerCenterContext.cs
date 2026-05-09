using Microsoft.EntityFrameworkCore;
using VolunteerCenter.Models;

namespace VolunteerCenter.Data
{
    public class VolunteerCenterContext : DbContext
    {
        public VolunteerCenterContext() { }

        public VolunteerCenterContext(DbContextOptions<VolunteerCenterContext> options) 
            : base(options) { }

        public DbSet<Volunteer> Volunteers { get; set; } = null!;
        public DbSet<Specialist> Specialists { get; set; } = null!;
        public DbSet<Beneficiary> Beneficiaries { get; set; } = null!;
        public DbSet<VolunteerBeneficiary> VolunteerBeneficiaries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Volunteer>()
                .HasOne(v => v.Curator)
                .WithMany(s => s.Volunteers)
                .HasForeignKey(v => v.CuratorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<VolunteerBeneficiary>()
                .HasKey(vb => new { vb.VolunteerId, vb.BeneficiaryId });

            modelBuilder.Entity<VolunteerBeneficiary>()
                .HasOne(vb => vb.Volunteer)
                .WithMany(v => v.VolunteerBeneficiaries)
                .HasForeignKey(vb => vb.VolunteerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VolunteerBeneficiary>()
                .HasOne(vb => vb.Beneficiary)
                .WithMany(b => b.VolunteerBeneficiaries)
                .HasForeignKey(vb => vb.BeneficiaryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Volunteer>()
                .HasIndex(v => v.Email)
                .IsUnique();
        }
    }
}