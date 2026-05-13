using ClinicApp.Application.Common;
using MediatR;

namespace ClinicApp.Application.Patients.Commands.CreatePatient;

public record CreatePatientCommand(
    string FirstName,
    string LastName,
    string IdentityNumber,
    string Email,
    string Phone,
    DateTime DateOfBirth) : IRequest<Result<PatientDto>>;
