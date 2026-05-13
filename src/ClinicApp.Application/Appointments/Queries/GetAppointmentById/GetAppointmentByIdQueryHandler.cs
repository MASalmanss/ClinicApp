using ClinicApp.Application.Common;
using ClinicApp.Application.Interfaces;
using MediatR;

namespace ClinicApp.Application.Appointments.Queries.GetAppointmentById;

public class GetAppointmentByIdQueryHandler
    : IRequestHandler<GetAppointmentByIdQuery, Result<AppointmentDto>>
{
    private readonly IUnitOfWork _uow;

    public GetAppointmentByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<AppointmentDto>> Handle(
        GetAppointmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var appointment = await _uow.Appointments.GetByIdAsync(request.Id, cancellationToken);

        if (appointment is null)
            return Error.NotFound($"Id={request.Id} olan randevu bulunamadı.");

        return new AppointmentDto(
            appointment.Id,
            appointment.PatientId,
            appointment.Patient.FullName,
            appointment.DoctorId,
            appointment.Doctor.FullName,
            appointment.Doctor.Specialization,
            appointment.ScheduledAt,
            appointment.Status,
            appointment.Notes);
    }
}
