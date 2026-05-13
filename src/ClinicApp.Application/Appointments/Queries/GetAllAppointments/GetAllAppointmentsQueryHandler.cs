using ClinicApp.Application.Common;
using ClinicApp.Application.Interfaces;
using MediatR;

namespace ClinicApp.Application.Appointments.Queries.GetAllAppointments;

public class GetAllAppointmentsQueryHandler
    : IRequestHandler<GetAllAppointmentsQuery, Result<IEnumerable<AppointmentDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAllAppointmentsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<AppointmentDto>>> Handle(
        GetAllAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        var appointments = await _uow.Appointments.GetAllAsync(cancellationToken);

        var dtos = appointments.Select(a => new AppointmentDto(
            a.Id,
            a.PatientId,
            a.Patient.FullName,
            a.DoctorId,
            a.Doctor.FullName,
            a.Doctor.Specialization,
            a.ScheduledAt,
            a.Status,
            a.Notes));

        return Result<IEnumerable<AppointmentDto>>.Ok(dtos);
    }
}
