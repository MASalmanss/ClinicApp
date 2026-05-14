using ClinicApp.Domain.Entities;
using ClinicApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        // Explicit ValueConverter — EF Core'un lambda'yı yanlış yorumlamasını önler
        var emailConverter = new ValueConverter<Email, string>(
            email => email.Value,
            dbValue => Email.FromDatabase(dbValue));

        builder.Property(d => d.Email)
            .HasConversion(emailConverter)
            .IsRequired()
            .HasMaxLength(300);

        builder.HasIndex(d => d.Email)
            .IsUnique();

        builder.HasMany(d => d.Appointments)
            .WithOne(a => a.Doctor)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(d => d.Appointments)
            .HasField("_appointments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
