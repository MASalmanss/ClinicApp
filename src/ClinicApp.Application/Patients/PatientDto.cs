namespace ClinicApp.Application.Patients;

public record PatientDto(
    int Id,
    string FullName,
    string IdentityNumber,
    string Email,
    string Phone,
    DateTime DateOfBirth);
