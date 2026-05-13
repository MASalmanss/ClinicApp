using FluentValidation;

namespace ClinicApp.Application.Appointments.Commands.BookAppointment;

public class BookAppointmentValidator : AbstractValidator<BookAppointmentCommand>
{
    public BookAppointmentValidator()
    {
        RuleFor(x => x.PatientId)
            .GreaterThan(0).WithMessage("Geçerli bir hasta seçiniz.");

        RuleFor(x => x.DoctorId)
            .GreaterThan(0).WithMessage("Geçerli bir doktor seçiniz.");

        RuleFor(x => x.ScheduledAt)
            .GreaterThan(DateTime.UtcNow).WithMessage("Randevu tarihi gelecekte olmalıdır.");
    }
}
