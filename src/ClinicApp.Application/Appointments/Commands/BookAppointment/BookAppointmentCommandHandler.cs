using ClinicApp.Application.Common;
using ClinicApp.Application.Interfaces;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.Exceptions;
using MediatR;

namespace ClinicApp.Application.Appointments.Commands.BookAppointment;

public class BookAppointmentCommandHandler
    : IRequestHandler<BookAppointmentCommand, Result<AppointmentDto>>
{
    private readonly IUnitOfWork _uow;

    public BookAppointmentCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<AppointmentDto>> Handle(
        BookAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        var patient = await _uow.Patients.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
            return Error.NotFound($"Id={request.PatientId} olan hasta bulunamadı.");

        var doctor = await _uow.Doctors.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor is null)
            return Error.NotFound($"Id={request.DoctorId} olan doktor bulunamadı.");

        if (!doctor.IsAvailableAt(request.ScheduledAt))
            return Error.Conflict($"Dr. {doctor.FullName} bu saatte başka bir randevusu var.");

        Appointment appointment;
        try
        {
            appointment = Appointment.Create(
                request.PatientId,
                request.DoctorId,
                request.ScheduledAt,
                request.Notes);
        }
        catch (DomainException ex)
        {
            return Error.Validation(ex.Message);
        }

        await _uow.Appointments.AddAsync(appointment, cancellationToken);
        await _uow.CommitAsync(cancellationToken);

        return new AppointmentDto(
            appointment.Id,
            patient.Id,
            patient.FullName,
            doctor.Id,
            doctor.FullName,
            doctor.Specialization,
            appointment.ScheduledAt,
            appointment.Status,
            appointment.Notes);
    }
}
