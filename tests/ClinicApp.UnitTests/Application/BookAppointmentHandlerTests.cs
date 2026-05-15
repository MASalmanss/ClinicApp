using ClinicApp.Application.Appointments.Commands.BookAppointment;
using ClinicApp.Application.Common;
using ClinicApp.Application.Interfaces;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace ClinicApp.UnitTests.Application;

public class BookAppointmentHandlerTests
{
    // Test için sahte nesneler
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IPatientRepository> _patientRepoMock = new();
    private readonly Mock<IDoctorRepository> _doctorRepoMock = new();
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock = new();
    private readonly BookAppointmentCommandHandler _handler;

    public BookAppointmentHandlerTests()
    {
        // UnitOfWork mock'una repository mock'larını bağla
        _uowMock.Setup(u => u.Patients).Returns(_patientRepoMock.Object);
        _uowMock.Setup(u => u.Doctors).Returns(_doctorRepoMock.Object);
        _uowMock.Setup(u => u.Appointments).Returns(_appointmentRepoMock.Object);

        _handler = new BookAppointmentCommandHandler(_uowMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithAppointmentDto()
    {
        // Arrange
        var patient = Patient.Create("Mehmet", "Demir", "12345678901",
            Email.Create("mehmet@test.com"),
            PhoneNumber.Create("05321234567"),
            new DateTime(1990, 1, 1));

        var doctor = Doctor.Create("Ahmet", "Yılmaz", "Kardiyoloji",
            Email.Create("ahmet@klinik.com"));

        var scheduledAt = DateTime.UtcNow.AddDays(5);

        _patientRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        _doctorRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctor);

        var command = new BookAppointmentCommand(1, 1, scheduledAt, "İlk muayene");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.PatientFullName.Should().Be("Mehmet Demir");
        result.Value.DoctorFullName.Should().Be("Dr. Ahmet Yılmaz");
        result.Value.ScheduledAt.Should().Be(scheduledAt);

        // Commit çağrıldı mı?
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PatientNotFound_ReturnsNotFoundError()
    {
        // Arrange — hasta yok
        _patientRepoMock
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient?)null);

        var command = new BookAppointmentCommand(99, 1, DateTime.UtcNow.AddDays(3), null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.NotFound);

        // Commit hiç çağrılmadı mı?
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DoctorNotFound_ReturnsNotFoundError()
    {
        // Arrange — hasta var, doktor yok
        var patient = Patient.Create("Mehmet", "Demir", "12345678901",
            Email.Create("mehmet@test.com"),
            PhoneNumber.Create("05321234567"),
            new DateTime(1990, 1, 1));

        _patientRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        _doctorRepoMock
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Doctor?)null);

        var command = new BookAppointmentCommand(1, 99, DateTime.UtcNow.AddDays(3), null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PastScheduledAt_ReturnsValidationError()
    {
        // Arrange
        var patient = Patient.Create("Mehmet", "Demir", "12345678901",
            Email.Create("mehmet@test.com"),
            PhoneNumber.Create("05321234567"),
            new DateTime(1990, 1, 1));

        var doctor = Doctor.Create("Ahmet", "Yılmaz", "Kardiyoloji",
            Email.Create("ahmet@klinik.com"));

        _patientRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        _doctorRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctor);

        // Geçmiş tarih
        var command = new BookAppointmentCommand(1, 1, DateTime.UtcNow.AddDays(-1), null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Validation);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
