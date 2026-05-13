using ClinicApp.Application.Common;
using ClinicApp.Application.Interfaces;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.ValueObjects;
using MediatR;

namespace ClinicApp.Application.Patients.Commands.CreatePatient;

public class CreatePatientCommandHandler
    : IRequestHandler<CreatePatientCommand, Result<PatientDto>>
{
    private readonly IUnitOfWork _uow;

    public CreatePatientCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PatientDto>> Handle(
        CreatePatientCommand request,
        CancellationToken cancellationToken)
    {
        // TC Kimlik No benzersiz mi?
        var existing = await _uow.Patients.GetByIdentityNumberAsync(request.IdentityNumber, cancellationToken);
        if (existing is not null)
            return Error.Conflict($"'{request.IdentityNumber}' kimlik numarasıyla kayıtlı hasta zaten var.");

        var patient = Patient.Create(
            request.FirstName,
            request.LastName,
            request.IdentityNumber,
            Email.Create(request.Email),
            PhoneNumber.Create(request.Phone),
            request.DateOfBirth);

        await _uow.Patients.AddAsync(patient, cancellationToken);
        await _uow.CommitAsync(cancellationToken);

        return new PatientDto(
            patient.Id,
            patient.FullName,
            patient.IdentityNumber,
            patient.Email.Value,
            patient.Phone.Value,
            patient.DateOfBirth);
    }
}
