using ClinicApp.Application.Common;
using MediatR;

namespace ClinicApp.Application.Doctors.Commands.CreateDoctor;

public record CreateDoctorCommand(
    string FirstName,
    string LastName,
    string Specialization,
    string Email) : IRequest<Result<DoctorDto>>;
