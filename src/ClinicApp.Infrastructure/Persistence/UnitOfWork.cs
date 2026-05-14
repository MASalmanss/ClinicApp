using ClinicApp.Application.Interfaces;
using ClinicApp.Infrastructure.Persistence.Repositories;

namespace ClinicApp.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IPatientRepository Patients { get; }
    public IDoctorRepository Doctors { get; }
    public IAppointmentRepository Appointments { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Patients = new PatientRepository(context);
        Doctors = new DoctorRepository(context);
        Appointments = new AppointmentRepository(context);
    }

    public async Task<int> CommitAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
