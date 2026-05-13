using ClinicApp.Application.Common;
using MediatR;

namespace ClinicApp.Application.Doctors.Queries.GetDoctorById;

public record GetDoctorByIdQuery(int Id) : IRequest<Result<DoctorDto>>;
