using ClinicApp.Application.Interfaces;
using ClinicApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Persistence.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _context;

    public PatientRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Patient>> GetAllAsync(CancellationToken ct = default)
        => await _context.Patients.ToListAsync(ct);

    public async Task<Patient?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Patients
            .Include(p => p.Appointments)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Patient?> GetByIdentityNumberAsync(string identityNumber, CancellationToken ct = default)
        => await _context.Patients
            .FirstOrDefaultAsync(p => p.IdentityNumber == identityNumber, ct);

    public async Task AddAsync(Patient patient, CancellationToken ct = default)
        => await _context.Patients.AddAsync(patient, ct);

    public void Delete(Patient patient)
        => _context.Patients.Remove(patient);
}
