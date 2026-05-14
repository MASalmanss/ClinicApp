using ClinicApp.Domain.Entities;
using ClinicApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicApp.Infrastructure.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Specialization)
            .IsRequired()
            .HasMaxLength(100);

        // Email value object → string sütuna dönüştür
        builder.Property(d => d.Email)
            .HasConversion(
                email => email.Value,
                value => Email.FromDatabase(value))
            .IsRequired()
            .HasMaxLength(300);

        builder.HasIndex(d => d.Email)
            .IsUnique();

        // Navigation
        builder.HasMany<Appointment>()
            .WithOne()
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict); // doktor silinince randevu silinmesin, hata versin
    }
}
