using ClinicApp.Application.Common;
using ClinicApp.Application.Interfaces;
using MediatR;

namespace ClinicApp.Application.Doctors.Queries.GetAllDoctors;

public class GetAllDoctorsQueryHandler
    : IRequestHandler<GetAllDoctorsQuery, Result<IEnumerable<DoctorDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAllDoctorsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<DoctorDto>>> Handle(
        GetAllDoctorsQuery request,
        CancellationToken cancellationToken)
    {
        var doctors = await _uow.Doctors.GetAllAsync(cancellationToken);

        var dtos = doctors.Select(d => new DoctorDto(
            d.Id,
            d.FullName,
            d.Specialization,
            d.Email.Value));

        return Result<IEnumerable<DoctorDto>>.Ok(dtos);
    }
}
