using FluentValidation;

namespace ClinicApp.Application.Patients.Commands.CreatePatient;

public class CreatePatientValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad boş olamaz.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad boş olamaz.")
            .MaximumLength(100);

        RuleFor(x => x.IdentityNumber)
            .NotEmpty().WithMessage("TC Kimlik numarası boş olamaz.")
            .Length(11).WithMessage("TC Kimlik numarası 11 haneli olmalıdır.")
            .Matches(@"^\d{11}$").WithMessage("TC Kimlik numarası yalnızca rakamlardan oluşmalıdır.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz.")
            .Matches(@"^(\+90|0)?5\d{9}$").WithMessage("Geçerli bir Türk telefon numarası giriniz.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow).WithMessage("Doğum tarihi geçmişte olmalıdır.")
            .GreaterThan(new DateTime(1900, 1, 1)).WithMessage("Geçerli bir doğum tarihi giriniz.");
    }
}
