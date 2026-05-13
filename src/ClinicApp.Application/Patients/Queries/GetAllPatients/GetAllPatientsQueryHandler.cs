using ClinicApp.Application.Common;
using ClinicApp.Application.Interfaces;
using MediatR;

namespace ClinicApp.Application.Patients.Queries.GetAllPatients;

public class GetAllPatientsQueryHandler
    : IRequestHandler<GetAllPatientsQuery, Result<IEnumerable<PatientDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAllPatientsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<IEnumerable<PatientDto>>> Handle(
        GetAllPatientsQuery request,
        CancellationToken cancellationToken)
    {
        var patients = await _uow.Patients.GetAllAsync(cancellationToken);

        var dtos = patients.Select(p => new PatientDto(
            p.Id,
            p.FullName,
            p.IdentityNumber,
            p.Email.Value,
            p.Phone.Value,
            p.DateOfBirth));

        return Result<IEnumerable<PatientDto>>.Ok(dtos);
    }
}
