using ClinicApp.Domain.Enums;

namespace ClinicApp.Application.Appointments;

public record AppointmentDto(
    int Id,
    int PatientId,
    string PatientFullName,
    int DoctorId,
    string DoctorFullName,
    string DoctorSpecialization,
    DateTime ScheduledAt,
    AppointmentStatus Status,
    string? Notes);
