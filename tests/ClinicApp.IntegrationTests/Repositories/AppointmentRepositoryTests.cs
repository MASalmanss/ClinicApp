using ClinicApp.Domain.Entities;
using ClinicApp.Domain.Enums;
using ClinicApp.Domain.ValueObjects;
using ClinicApp.Infrastructure.Persistence;
using ClinicApp.Infrastructure.Persistence.Repositories;
using FluentAssertions;

namespace ClinicApp.IntegrationTests.Repositories;

public class AppointmentRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AppointmentRepository _appointmentRepo;
    private readonly PatientRepository _patientRepo;
    private readonly DoctorRepository _doctorRepo;

    public AppointmentRepositoryTests()
    {
        _context = TestDbContextFactory.Create();
        _appointmentRepo = new AppointmentRepository(_context);
        _patientRepo = new PatientRepository(_context);
        _doctorRepo = new DoctorRepository(_context);
    }

    public void Dispose() => TestDbContextFactory.Destroy(_context);

    // ─── Yardımcılar ───

    private async Task<(Patient patient, Doctor doctor)> SeedPatientAndDoctor()
    {
        var patient = Patient.Create("Mehmet", "Demir", "12345678901",
            Email.Create("mehmet@test.com"),
            PhoneNumber.Create("05321234567"),
            new DateTime(1990, 1, 1));

        var doctor = Doctor.Create("Ahmet", "Yılmaz", "Kardiyoloji",
            Email.Create("ahmet@klinik.com"));

        await _patientRepo.AddAsync(patient);
        await _doctorRepo.AddAsync(doctor);
        await _context.SaveChangesAsync();

        return (patient, doctor);
    }

    // ─── Testler ───

    [Fact]
    public async Task AddAsync_ThenGetById_ReturnsAppointmentWithNavigations()
    {
        var (patient, doctor) = await SeedPatientAndDoctor();
        var scheduledAt = DateTime.UtcNow.AddDays(5);

        var appointment = Appointment.Create(patient.Id, doctor.Id, scheduledAt, "Test notu");
        await _appointmentRepo.AddAsync(appointment);
        await _context.SaveChangesAsync();

        var found = await _appointmentRepo.GetByIdAsync(appointment.Id);

        found.Should().NotBeNull();
        found!.Status.Should().Be(AppointmentStatus.Pending);
        found.Notes.Should().Be("Test notu");
        found.Patient.FullName.Should().Be("Mehmet Demir");
        found.Doctor.FullName.Should().Be("Dr. Ahmet Yılmaz");
    }

    [Fact]
    public async Task Update_CancelAppointment_PersistsStatusChange()
    {
        var (patient, doctor) = await SeedPatientAndDoctor();

        var appointment = Appointment.Create(patient.Id, doctor.Id, DateTime.UtcNow.AddDays(5));
        await _appointmentRepo.AddAsync(appointment);
        await _context.SaveChangesAsync();

        appointment.Cancel();
        _appointmentRepo.Update(appointment);
        await _context.SaveChangesAsync();

        var found = await _appointmentRepo.GetByIdAsync(appointment.Id);
        found!.Status.Should().Be(AppointmentStatus.Cancelled);
    }

    [Fact]
    public async Task GetByPatientIdAsync_ReturnsOnlyThatPatientsAppointments()
    {
        var (patient, doctor) = await SeedPatientAndDoctor();

        // 2 randevu aynı hastaya
        await _appointmentRepo.AddAsync(Appointment.Create(patient.Id, doctor.Id, DateTime.UtcNow.AddDays(3)));
        await _appointmentRepo.AddAsync(Appointment.Create(patient.Id, doctor.Id, DateTime.UtcNow.AddDays(6)));
        await _context.SaveChangesAsync();

        var appointments = await _appointmentRepo.GetByPatientIdAsync(patient.Id);

        appointments.Should().HaveCount(2);
        appointments.Should().AllSatisfy(a => a.PatientId.Should().Be(patient.Id));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAppointmentsWithNavigations()
    {
        var (patient, doctor) = await SeedPatientAndDoctor();
        await _appointmentRepo.AddAsync(Appointment.Create(patient.Id, doctor.Id, DateTime.UtcNow.AddDays(3)));
        await _context.SaveChangesAsync();

        var all = await _appointmentRepo.GetAllAsync();

        all.Should().HaveCount(1);
        all.First().Patient.Should().NotBeNull();
        all.First().Doctor.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_ConfirmAppointment_StatusBecomesConfirmed()
    {
        var (patient, doctor) = await SeedPatientAndDoctor();

        var appointment = Appointment.Create(patient.Id, doctor.Id, DateTime.UtcNow.AddDays(5));
        await _appointmentRepo.AddAsync(appointment);
        await _context.SaveChangesAsync();

        appointment.Confirm();
        _appointmentRepo.Update(appointment);
        await _context.SaveChangesAsync();

        var found = await _appointmentRepo.GetByIdAsync(appointment.Id);
        found!.Status.Should().Be(AppointmentStatus.Confirmed);
    }
}
