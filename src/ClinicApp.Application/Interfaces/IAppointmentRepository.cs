using ClinicApp.Domain.Entities;

namespace ClinicApp.Application.Interfaces;

public interface IAppointmentRepository
{
    Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken ct = default);
    Task<Appointment?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, CancellationToken ct = default);
    Task AddAsync(Appointment appointment, CancellationToken ct = default);
    void Update(Appointment appointment);
}
