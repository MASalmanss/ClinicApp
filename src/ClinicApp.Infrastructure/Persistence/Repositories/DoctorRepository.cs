using ClinicApp.Application.Interfaces;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Persistence.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly AppDbContext _context;

    public DoctorRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Doctor>> GetAllAsync(CancellationToken ct = default)
        => await _context.Doctors.ToListAsync(ct);

    public async Task<Doctor?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Doctors
            .Include(d => d.Appointments) // IsAvailableAt() için appointments gerekli
            .FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<Doctor?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        // Email nesnesi üzerinden karşılaştır — EF Core converter'ı doğru uygular
        var emailObj = Email.Create(email);
        return await _context.Doctors
            .FirstOrDefaultAsync(d => d.Email == emailObj, ct);
    }

    public async Task AddAsync(Doctor doctor, CancellationToken ct = default)
        => await _context.Doctors.AddAsync(doctor, ct);

    public void Delete(Doctor doctor)
        => _context.Doctors.Remove(doctor);
}
