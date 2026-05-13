using ClinicApp.Domain.Common;
using ClinicApp.Domain.Enums;
using ClinicApp.Domain.Exceptions;

namespace ClinicApp.Domain.Entities;

public class Appointment : BaseEntity
{
    public int PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;

    public int DoctorId { get; private set; }
    public Doctor Doctor { get; private set; } = null!;

    public DateTime ScheduledAt { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string? Notes { get; private set; }

    private Appointment() { }

    public static Appointment Create(int patientId, int doctorId, DateTime scheduledAt, string? notes = null)
    {
        if (scheduledAt <= DateTime.UtcNow)
            throw new DomainException("Randevu tarihi gelecekte olmalıdır.");

        return new Appointment
        {
            PatientId = patientId,
            DoctorId = doctorId,
            ScheduledAt = scheduledAt,
            Status = AppointmentStatus.Pending,
            Notes = notes
        };
    }

    public void Confirm()
    {
        if (Status != AppointmentStatus.Pending)
            throw new DomainException("Yalnızca beklemedeki randevular onaylanabilir.");

        Status = AppointmentStatus.Confirmed;
    }

    public void Complete()
    {
        if (Status != AppointmentStatus.Confirmed)
            throw new DomainException("Yalnızca onaylanmış randevular tamamlanabilir.");

        Status = AppointmentStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == AppointmentStatus.Completed)
            throw new DomainException("Tamamlanmış randevu iptal edilemez.");

        if (Status == AppointmentStatus.Cancelled)
            throw new DomainException("Randevu zaten iptal edilmiş.");

        if (ScheduledAt <= DateTime.UtcNow.AddHours(24))
            throw new DomainException("Randevu 24 saatten az kaldığında iptal edilemez.");

        Status = AppointmentStatus.Cancelled;
    }
}
