using ClinicApp.Application.Common;
using ClinicApp.Application.Interfaces;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.ValueObjects;
using MediatR;

namespace ClinicApp.Application.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommandHandler
    : IRequestHandler<CreateDoctorCommand, Result<DoctorDto>>
{
    private readonly IUnitOfWork _uow;

    public CreateDoctorCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<DoctorDto>> Handle(
        CreateDoctorCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await _uow.Doctors.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            return Error.Conflict($"'{request.Email}' e-postasıyla kayıtlı doktor zaten var.");

        var doctor = Doctor.Create(
            request.FirstName,
            request.LastName,
            request.Specialization,
            Email.Create(request.Email));

        await _uow.Doctors.AddAsync(doctor, cancellationToken);
        await _uow.CommitAsync(cancellationToken);

        return new DoctorDto(
            doctor.Id,
            doctor.FullName,
            doctor.Specialization,
            doctor.Email.Value);
    }
}
