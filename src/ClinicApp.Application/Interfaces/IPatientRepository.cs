using ClinicApp.Domain.Entities;

namespace ClinicApp.Application.Interfaces;

public interface IPatientRepository
{
    Task<IEnumerable<Patient>> GetAllAsync(CancellationToken ct = default);
    Task<Patient?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Patient?> GetByIdentityNumberAsync(string identityNumber, CancellationToken ct = default);
    Task AddAsync(Patient patient, CancellationToken ct = default);
    void Delete(Patient patient);
}
