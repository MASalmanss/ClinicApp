namespace ClinicApp.Application.Common;

public enum ErrorType
{
    Validation,  // 422 — kullanıcı yanlış veri gönderdi
    NotFound,    // 404 — kayıt bulunamadı
    Conflict,    // 409 — zaten var
    Unexpected   // 500 — beklenmedik hata
}
