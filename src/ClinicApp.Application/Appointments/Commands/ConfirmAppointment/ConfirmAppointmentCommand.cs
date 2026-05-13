using ClinicApp.Application.Common;
using MediatR;

namespace ClinicApp.Application.Appointments.Commands.ConfirmAppointment;

public record ConfirmAppointmentCommand(int AppointmentId) : IRequest<Result>;
