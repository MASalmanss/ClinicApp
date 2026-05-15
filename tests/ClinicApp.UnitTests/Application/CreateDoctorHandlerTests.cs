using ClinicApp.Application.Common;
using ClinicApp.Application.Doctors.Commands.CreateDoctor;
using ClinicApp.Application.Interfaces;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace ClinicApp.UnitTests.Application;

public class CreateDoctorHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IDoctorRepository> _doctorRepoMock = new();
    private readonly CreateDoctorCommandHandler _handler;

    public CreateDoctorHandlerTests()
    {
        _uowMock.Setup(u => u.Doctors).Returns(_doctorRepoMock.Object);
        _handler = new CreateDoctorCommandHandler(_uowMock.Object);
    }

    [Fact]
    public async Task Handle_NewEmail_CreatesDoctorAndReturnsDto()
    {
        // Arrange — bu email ile doktor yok
        _doctorRepoMock
            .Setup(r => r.GetByEmailAsync("ahmet@klinik.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Doctor?)null);

        var command = new CreateDoctorCommand("Ahmet", "Yılmaz", "Kardiyoloji", "ahmet@klinik.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.FullName.Should().Be("Dr. Ahmet Yılmaz");
        result.Value.Specialization.Should().Be("Kardiyoloji");
        result.Value.Email.Should().Be("ahmet@klinik.com");

        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ReturnsConflictError()
    {
        // Arrange — bu email zaten kayıtlı
        var existingDoctor = Doctor.Create("Ali", "Veli", "Nöroloji",
            Email.Create("ahmet@klinik.com"));

        _doctorRepoMock
            .Setup(r => r.GetByEmailAsync("ahmet@klinik.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingDoctor);

        var command = new CreateDoctorCommand("Mehmet", "Can", "Göz", "ahmet@klinik.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Conflict);

        // Kayıt yapılmadı
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
