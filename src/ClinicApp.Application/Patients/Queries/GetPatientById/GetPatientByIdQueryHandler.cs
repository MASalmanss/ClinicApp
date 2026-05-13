using ClinicApp.Application.Common;
using ClinicApp.Application.Interfaces;
using MediatR;

namespace ClinicApp.Application.Patients.Queries.GetPatientById;

public class GetPatientByIdQueryHandler
    : IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>
{
    private readonly IUnitOfWork _uow;

    public GetPatientByIdQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PatientDto>> Handle(
        GetPatientByIdQuery request,
        CancellationToken cancellationToken)
    {
        var patient = await _uow.Patients.GetByIdAsync(request.Id, cancellationToken);

        if (patient is null)
            return Error.NotFound($"Patient with id {request.Id} not found.");

        return new PatientDto(
            patient.Id,
            patient.FullName,
            patient.IdentityNumber,
            patient.Email.Value,
            patient.Phone.Value,
            patient.DateOfBirth);
    }
}
