namespace ClinicApp.Domain.Exceptions;

public class DataCorruptionException : Exception
{
    public DataCorruptionException(string field, string value)
        : base($"Veritabanından okunan '{field}' alanı geçersiz bir değer içeriyor: '{value}'. Bu bir veri bütünlüğü sorunudur.") { }
}
