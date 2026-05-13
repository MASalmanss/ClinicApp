using ClinicApp.Domain.Common;
using ClinicApp.Domain.Exceptions;
using ClinicApp.Domain.ValueObjects;

namespace ClinicApp.Domain.Entities;

public class Doctor : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Specialization { get; private set; } = string.Empty; // Uzmanlık alanı
    public Email Email { get; private set; } = null!;

    private readonly List<Appointment> _appointments = [];
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    private Doctor() { }

    public static Doctor Create(string firstName, string lastName, string specialization, Email email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("Ad boş olamaz.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Soyad boş olamaz.");

        if (string.IsNullOrWhiteSpace(specialization))
            throw new DomainException("Uzmanlık alanı boş olamaz.");

        return new Doctor
        {
            FirstName = firstName,
            LastName = lastName,
            Specialization = specialization,
            Email = email
        };
    }

    public string FullName => $"Dr. {FirstName} {LastName}";

    // Doktorun belirli bir saatte müsait olup olmadığını kontrol eder
    public bool IsAvailableAt(DateTime scheduledAt)
        => !_appointments.Any(a =>
            a.ScheduledAt == scheduledAt &&
            a.Status is not Enums.AppointmentStatus.Cancelled);
}
