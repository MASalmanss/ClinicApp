using ClinicApp.Application.Common;
using MediatR;

namespace ClinicApp.Application.Doctors.Queries.GetAllDoctors;

public record GetAllDoctorsQuery : IRequest<Result<IEnumerable<DoctorDto>>>;
