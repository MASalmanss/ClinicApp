using System.Text.RegularExpressions;
using ClinicApp.Domain.Exceptions;

namespace ClinicApp.Domain.ValueObjects;

public sealed record PhoneNumber
{
    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Telefon numarası boş olamaz.");

        // Boşluk ve tire temizle, sonra kontrol et
        var cleaned = value.Replace(" ", "").Replace("-", "");

        if (!Regex.IsMatch(cleaned, @"^(\+90|0)?5\d{9}$"))
            throw new DomainException("Geçersiz telefon numarası. Örnek: 05321234567");

        return new PhoneNumber(cleaned);
    }

    // DB'den gelen veri — altyapı hatası
    public static PhoneNumber FromDatabase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DataCorruptionException(nameof(PhoneNumber), value);

        var cleaned = value.Replace(" ", "").Replace("-", "");

        if (!Regex.IsMatch(cleaned, @"^(\+90|0)?5\d{9}$"))
            throw new DataCorruptionException(nameof(PhoneNumber), value);

        return new PhoneNumber(cleaned);
    }
}
