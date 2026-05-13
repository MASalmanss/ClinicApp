using ClinicApp.Application.Common;
using MediatR;

namespace ClinicApp.Application.Patients.Queries.GetPatientById;

public record GetPatientByIdQuery(int Id) : IRequest<Result<PatientDto>>;
