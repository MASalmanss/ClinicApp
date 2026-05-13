using ClinicApp.Application.Common;
using MediatR;

namespace ClinicApp.Application.Appointments.Commands.CancelAppointment;

public record CancelAppointmentCommand(int AppointmentId) : IRequest<Result>;
