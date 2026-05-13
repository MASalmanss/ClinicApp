using ClinicApp.Application.Common;
using MediatR;

namespace ClinicApp.Application.Appointments.Queries.GetAppointmentById;

public record GetAppointmentByIdQuery(int Id) : IRequest<Result<AppointmentDto>>;
