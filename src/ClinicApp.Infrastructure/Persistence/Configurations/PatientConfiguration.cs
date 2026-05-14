using ClinicApp.Domain.Entities;
using ClinicApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicApp.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.IdentityNumber)
            .IsRequired()
            .HasMaxLength(11);

        builder.HasIndex(p => p.IdentityNumber)
            .IsUnique();

        // Email value object → string sütuna dönüştür
        builder.Property(p => p.Email)
            .HasConversion(
                email => email.Value,
                value => Email.FromDatabase(value))
            .IsRequired()
            .HasMaxLength(300);

        builder.HasIndex(p => p.Email)
            .IsUnique();

        // PhoneNumber value object → string sütuna dönüştür
        builder.Property(p => p.Phone)
            .HasConversion(
                phone => phone.Value,
                value => PhoneNumber.FromDatabase(value))
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(p => p.DateOfBirth)
            .IsRequired();

        // Navigation — silinirse randevular da silinsin
        builder.HasMany<Appointment>()
            .WithOne()
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
