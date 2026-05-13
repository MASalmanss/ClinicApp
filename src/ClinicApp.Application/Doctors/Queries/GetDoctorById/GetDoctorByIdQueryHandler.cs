using ClinicApp.Application.Common;
using ClinicApp.Application.Interfaces;
using MediatR;

namespace ClinicApp.Application.Doctors.Queries.GetDoctorById;

public class GetDoctorByIdQueryHandler
    : IRequestHandler<GetDoctorByIdQuery, Result<DoctorDto>>
{
    private readonly IUnitOfWork _uow;

    public GetDoctorByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<DoctorDto>> Handle(
        GetDoctorByIdQuery request,
        CancellationToken cancellationToken)
    {
        var doctor = await _uow.Doctors.GetByIdAsync(request.Id, cancellationToken);

        if (doctor is null)
            return Error.NotFound($"Doctor with id {request.Id} not found.");

        return new DoctorDto(
            doctor.Id,
            doctor.FullName,
            doctor.Specialization,
            doctor.Email.Value);
    }
}
