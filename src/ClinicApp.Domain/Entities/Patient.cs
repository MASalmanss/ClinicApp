using ClinicApp.Domain.Common;
using ClinicApp.Domain.Exceptions;
using ClinicApp.Domain.ValueObjects;

namespace ClinicApp.Domain.Entities;

public class Patient : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string IdentityNumber { get; private set; } = string.Empty; // TC Kimlik No
    public Email Email { get; private set; } = null!;
    public PhoneNumber Phone { get; private set; } = null!;
    public DateTime DateOfBirth { get; private set; }

    private readonly List<Appointment> _appointments = [];
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    private Patient() { }

    public static Patient Create(
        string firstName,
        string lastName,
        string identityNumber,
        Email email,
        PhoneNumber phone,
        DateTime dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("Ad boş olamaz.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Soyad boş olamaz.");

        if (string.IsNullOrWhiteSpace(identityNumber) || identityNumber.Length != 11)
            throw new DomainException("TC Kimlik No 11 haneli olmalıdır.");

        if (dateOfBirth >= DateTime.UtcNow)
            throw new DomainException("Doğum tarihi geçmişte olmalıdır.");

        return new Patient
        {
            FirstName = firstName,
            LastName = lastName,
            IdentityNumber = identityNumber,
            Email = email,
            Phone = phone,
            DateOfBirth = dateOfBirth
        };
    }

    public string FullName => $"{FirstName} {LastName}";
}
