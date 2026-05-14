using ClinicApp.Application.Interfaces;
using ClinicApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Persistence.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _context;

    public AppointmentRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken ct = default)
        => await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .ToListAsync(ct);

    public async Task<Appointment?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, CancellationToken ct = default)
        => await _context.Appointments
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId)
            .ToListAsync(ct);

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, CancellationToken ct = default)
        => await _context.Appointments
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId)
            .ToListAsync(ct);

    public async Task AddAsync(Appointment appointment, CancellationToken ct = default)
        => await _context.Appointments.AddAsync(appointment, ct);

    public void Update(Appointment appointment)
        => _context.Appointments.Update(appointment);
}
