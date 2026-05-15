using ClinicApp.Domain.Entities;
using ClinicApp.Domain.Enums;
using ClinicApp.Domain.Exceptions;
using FluentAssertions;

namespace ClinicApp.UnitTests.Domain;

public class AppointmentTests
{
    // ───── Create ─────

    [Fact]
    public void Create_ValidData_ReturnsAppointmentWithPendingStatus()
    {
        var scheduledAt = DateTime.UtcNow.AddDays(3);

        var appointment = Appointment.Create(patientId: 1, doctorId: 1, scheduledAt);

        appointment.Status.Should().Be(AppointmentStatus.Pending);
        appointment.PatientId.Should().Be(1);
        appointment.DoctorId.Should().Be(1);
        appointment.ScheduledAt.Should().Be(scheduledAt);
    }

    [Fact]
    public void Create_PastDate_ThrowsDomainException()
    {
        var pastDate = DateTime.UtcNow.AddDays(-1);

        Action act = () => Appointment.Create(1, 1, pastDate);

        act.Should().Throw<DomainException>()
           .WithMessage("*gelecekte*");
    }

    // ───── Confirm ─────

    [Fact]
    public void Confirm_WhenPending_ChangesStatusToConfirmed()
    {
        var appointment = Appointment.Create(1, 1, DateTime.UtcNow.AddDays(3));

        appointment.Confirm();

        appointment.Status.Should().Be(AppointmentStatus.Confirmed);
    }

    [Fact]
    public void Confirm_WhenAlreadyConfirmed_ThrowsDomainException()
    {
        var appointment = Appointment.Create(1, 1, DateTime.UtcNow.AddDays(3));
        appointment.Confirm();

        Action act = () => appointment.Confirm();

        act.Should().Throw<DomainException>()
           .WithMessage("*beklemedeki*");
    }

    [Fact]
    public void Confirm_WhenCancelled_ThrowsDomainException()
    {
        var appointment = Appointment.Create(1, 1, DateTime.UtcNow.AddDays(3));
        appointment.Cancel();

        Action act = () => appointment.Confirm();

        act.Should().Throw<DomainException>();
    }

    // ───── Cancel ─────

    [Fact]
    public void Cancel_WhenPendingAndMoreThan24HoursAway_ChangesStatusToCancelled()
    {
        var appointment = Appointment.Create(1, 1, DateTime.UtcNow.AddDays(3));

        appointment.Cancel();

        appointment.Status.Should().Be(AppointmentStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenLessThan24HoursAway_ThrowsDomainException()
    {
        var appointment = Appointment.Create(1, 1, DateTime.UtcNow.AddHours(12));

        Action act = () => appointment.Cancel();

        act.Should().Throw<DomainException>()
           .WithMessage("*24 saat*");
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ThrowsDomainException()
    {
        var appointment = Appointment.Create(1, 1, DateTime.UtcNow.AddDays(3));
        appointment.Cancel();

        Action act = () => appointment.Cancel();

        act.Should().Throw<DomainException>()
           .WithMessage("*zaten iptal*");
    }

    [Fact]
    public void Cancel_WhenCompleted_ThrowsDomainException()
    {
        var appointment = Appointment.Create(1, 1, DateTime.UtcNow.AddDays(3));
        appointment.Confirm();
        appointment.Complete();

        Action act = () => appointment.Cancel();

        act.Should().Throw<DomainException>()
           .WithMessage("*Tamamlanmış*");
    }

    // ───── Complete ─────

    [Fact]
    public void Complete_WhenConfirmed_ChangesStatusToCompleted()
    {
        var appointment = Appointment.Create(1, 1, DateTime.UtcNow.AddDays(3));
        appointment.Confirm();

        appointment.Complete();

        appointment.Status.Should().Be(AppointmentStatus.Completed);
    }

    [Fact]
    public void Complete_WhenPending_ThrowsDomainException()
    {
        var appointment = Appointment.Create(1, 1, DateTime.UtcNow.AddDays(3));

        Action act = () => appointment.Complete();

        act.Should().Throw<DomainException>()
           .WithMessage("*onaylanmış*");
    }
}
