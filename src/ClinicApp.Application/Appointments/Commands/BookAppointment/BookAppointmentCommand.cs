using ClinicApp.Application.Common;
using MediatR;

namespace ClinicApp.Application.Appointments.Commands.BookAppointment;

public record BookAppointmentCommand(
    int PatientId,
    int DoctorId,
    DateTime ScheduledAt,
    string? Notes) : IRequest<Result<AppointmentDto>>;
