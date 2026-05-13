namespace ClinicApp.Application.Doctors;

public record DoctorDto(
    int Id,
    string FullName,
    string Specialization,
    string Email);
