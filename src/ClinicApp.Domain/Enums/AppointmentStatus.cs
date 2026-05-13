namespace ClinicApp.Domain.Enums;

public enum AppointmentStatus
{
    Pending = 1,    // Oluşturuldu, onay bekliyor
    Confirmed = 2,  // Doktor onayladı
    Completed = 3,  // Gerçekleşti
    Cancelled = 4   // İptal edildi
}
