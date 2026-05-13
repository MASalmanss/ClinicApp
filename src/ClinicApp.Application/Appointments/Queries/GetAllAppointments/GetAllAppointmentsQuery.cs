using ClinicApp.Application.Common;
using MediatR;

namespace ClinicApp.Application.Appointments.Queries.GetAllAppointments;

public record GetAllAppointmentsQuery : IRequest<Result<IEnumerable<AppointmentDto>>>;
