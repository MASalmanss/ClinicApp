using ClinicApp.Domain.Entities;

namespace ClinicApp.Application.Interfaces;

public interface IDoctorRepository
{
    Task<IEnumerable<Doctor>> GetAllAsync(CancellationToken ct = default);
    Task<Doctor?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Doctor?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(Doctor doctor, CancellationToken ct = default);
    void Delete(Doctor doctor);
}
