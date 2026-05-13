using ClinicApp.Application.Common;
using ClinicApp.Application.Interfaces;
using ClinicApp.Domain.Exceptions;
using MediatR;

namespace ClinicApp.Application.Appointments.Commands.ConfirmAppointment;

public class ConfirmAppointmentCommandHandler
    : IRequestHandler<ConfirmAppointmentCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public ConfirmAppointmentCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(
        ConfirmAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        var appointment = await _uow.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken);

        if (appointment is null)
            return Error.NotFound($"Id={request.AppointmentId} olan randevu bulunamadı.");

        try
        {
            appointment.Confirm();
        }
        catch (DomainException ex)
        {
            return Error.Validation(ex.Message);
        }

        _uow.Appointments.Update(appointment);
        await _uow.CommitAsync(cancellationToken);

        return Result.Ok();
    }
}
