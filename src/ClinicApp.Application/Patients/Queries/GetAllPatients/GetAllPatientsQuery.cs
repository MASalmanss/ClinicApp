using ClinicApp.Application.Common;
using MediatR;

namespace ClinicApp.Application.Patients.Queries.GetAllPatients;

// IRequest<T> — MediatR'a "bu bir istek, T tipinde cevap bekliyorum" der
public record GetAllPatientsQuery : IRequest<Result<IEnumerable<PatientDto>>>;
