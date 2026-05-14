using ClinicApp.Domain.Entities;
using ClinicApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        var emailConverter = new ValueConverter<Email, string>(
            email => email.Value,
            dbValue => Email.FromDatabase(dbValue));

        builder.Property(p => p.Email)
            .HasConversion(emailConverter)
            .IsRequired()
            .HasMaxLength(300);

        builder.HasIndex(p => p.Email)
            .IsUnique();

        var phoneConverter = new ValueConverter<PhoneNumber, string>(
            phone => phone.Value,
            dbValue => PhoneNumber.FromDatabase(dbValue));

        builder.Property(p => p.Phone)
            .HasConversion(phoneConverter)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(p => p.DateOfBirth)
            .IsRequired();

        builder.HasMany(p => p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Appointments)
            .HasField("_appointments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
