using System.Text.RegularExpressions;
using ClinicApp.Domain.Exceptions;

namespace ClinicApp.Domain.ValueObjects;

public sealed record Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    // Kullanıcıdan gelen veri — iş kuralı hatası
    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("E-posta boş olamaz.");

        if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new DomainException("Geçersiz e-posta formatı.");

        if (value.Length > 300)
            throw new DomainException("E-posta en fazla 300 karakter olabilir.");

        return new Email(value.ToLowerInvariant());
    }

    // DB'den gelen veri — altyapı hatası
    public static Email FromDatabase(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new DataCorruptionException(nameof(Email), value);

        return new Email(value);
    }
}
