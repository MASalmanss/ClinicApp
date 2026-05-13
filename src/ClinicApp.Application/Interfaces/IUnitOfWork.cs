namespace ClinicApp.Application.Interfaces;

public interface IUnitOfWork
{
    IPatientRepository Patients { get; }
    IDoctorRepository Doctors { get; }
    IAppointmentRepository Appointments { get; }

    Task<int> CommitAsync(CancellationToken ct = default);
}
