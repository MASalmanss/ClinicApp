# ClinicApp — Hasta Randevu Sistemi / Patient Appointment System

> **TR** | [EN](#en-patient-appointment-system)

---

## TR — Hasta Randevu Sistemi

Doktor, hasta ve randevu yönetimi için geliştirilmiş bir **ASP.NET Core Web API** projesidir.  
Amaç: **Clean Architecture**, **CQRS**, **MediatR** ve **Domain-Driven Design** kavramlarını gerçek bir senaryo üzerinde uygulamalı olarak öğrenmek.

---

### Mimari

```
ClinicApp/
├── src/
│   ├── ClinicApp.Domain          # İş kuralları, entity'ler, value object'ler
│   ├── ClinicApp.Application     # CQRS handler'ları, pipeline behavior'ları, validator'lar
│   ├── ClinicApp.Infrastructure  # EF Core, repository implementasyonları, UnitOfWork
│   └── ClinicApp.Api             # Controller'lar, middleware, Program.cs
└── tests/
    ├── ClinicApp.UnitTests        # Domain ve handler unit testleri (Moq)
    └── ClinicApp.IntegrationTests # Repository integration testleri (SQLite)
```

**Bağımlılık yönü:** `Api → Application → Domain` ← `Infrastructure`  
Her katman yalnızca içteki katmanı bilir. Domain hiçbir şeye bağımlı değildir.

---

### Kullanılan Teknolojiler ve Kavramlar

| Katman | Teknoloji / Kavram |
|--------|-------------------|
| Domain | Rich Domain Model, Value Objects (`Email`, `PhoneNumber`), DomainException, DataCorruptionException |
| Application | CQRS, MediatR 12, Result Pattern, FluentValidation, Pipeline Behaviors |
| Infrastructure | EF Core 8, SQLite, Fluent API konfigürasyonları, Value Converters, Repository Pattern, Unit of Work |
| Api | ASP.NET Core 8, Global Exception Middleware, Swagger, Serilog |
| Test | xUnit, Moq, FluentAssertions, SQLite In-Memory |

---

### Domain Katmanı

#### Entity'ler

**`Patient`** — Ad, soyad, TC kimlik numarası (11 hane), e-posta, telefon, doğum tarihi  
**`Doctor`** — Ad, soyad, uzmanlık alanı, e-posta. `IsAvailableAt(DateTime)` metodu ile belirli saatte müsaitlik kontrolü yapar  
**`Appointment`** — Hasta + doktor + tarih + durum. Durum geçişleri domain iş kurallarına tabidir:

```
Pending → Confirmed → Completed
Pending → Cancelled
Confirmed → Cancelled  (24 saatten az kalmışsa iptal edilemez)
Completed → ❌  (iptal edilemez, geri alınamaz)
```

#### Value Objects

`Email.Create()` → kullanıcıdan gelen input, geçersizse `DomainException`  
`Email.FromDatabase()` → DB'den okunan değer, geçersizse `DataCorruptionException`  

Bu ayrım, kullanıcı hatasını altyapı hatasından ayırt eder.

---

### CQRS Yapısı

```
Query  = Sadece okuma, yan etkisiz, veri döndürür
Command = Yazma, iş kuralı içerir, DB'yi değiştirir
```

Her özellik kendi klasöründe bulunur:

```
Application/
├── Doctors/
│   ├── DoctorDto.cs
│   ├── Queries/
│   │   ├── GetAllDoctors/       → GetAllDoctorsQuery + Handler
│   │   └── GetDoctorById/       → GetDoctorByIdQuery + Handler
│   └── Commands/
│       └── CreateDoctor/        → CreateDoctorCommand + Handler + Validator
├── Patients/  (aynı yapı)
└── Appointments/
    ├── Queries/  (GetAll, GetById)
    └── Commands/ (BookAppointment, ConfirmAppointment, CancelAppointment)
```

---

### Pipeline Behaviors

MediatR pipeline'ında her handler çalışmadan önce araya giren middleware katmanları:

```
İstek → LoggingBehavior → ValidationBehavior → Handler → Yanıt
```

**`LoggingBehavior`** — Her isteğin adını ve süresini loglar  
**`ValidationBehavior`** — FluentValidation çalıştırır. Hata varsa handler'a hiç gitmez, `Result.Fail` döndürür

---

### Result Pattern

Exception fırlatmak yerine sonuçlar `Result<T>` nesnesiyle taşınır:

```csharp
// Handler'da:
if (existing is not null)
    return Error.Conflict("Bu e-posta zaten kayıtlı.");

// Controller'da:
var result = await _mediator.Send(command);
return result.ToActionResult(); // otomatik HTTP kodu seçimi
```

| ErrorType | HTTP Kodu |
|-----------|-----------|
| NotFound | 404 |
| Conflict | 409 |
| Validation | 400 |
| Unexpected | 500 |

---

### API Endpoint'leri

#### Hastalar

| Method | URL | Açıklama |
|--------|-----|----------|
| `GET` | `/api/patients` | Tüm hastaları listele |
| `GET` | `/api/patients/{id}` | ID'ye göre hasta getir |
| `POST` | `/api/patients` | Yeni hasta oluştur |

#### Doktorlar

| Method | URL | Açıklama |
|--------|-----|----------|
| `GET` | `/api/doctors` | Tüm doktorları listele |
| `GET` | `/api/doctors/{id}` | ID'ye göre doktor getir |
| `POST` | `/api/doctors` | Yeni doktor oluştur |

#### Randevular

| Method | URL | Açıklama |
|--------|-----|----------|
| `GET` | `/api/appointments` | Tüm randevuları listele |
| `GET` | `/api/appointments/{id}` | ID'ye göre randevu getir |
| `POST` | `/api/appointments` | Yeni randevu oluştur |
| `PUT` | `/api/appointments/{id}/confirm` | Randevuyu onayla |
| `PUT` | `/api/appointments/{id}/cancel` | Randevuyu iptal et |

---

### Kurulum ve Çalıştırma

**Gereksinimler:** .NET 8 SDK

```bash
# Repoyu klonla
git clone https://github.com/MASalmanss/ClinicApp.git
cd ClinicApp

# Bağımlılıkları yükle
dotnet restore

# API'yi çalıştır (migration otomatik uygulanır)
dotnet run --project src/ClinicApp.Api --launch-profile http
```

Swagger: [http://localhost:5130/swagger](http://localhost:5130/swagger)

---

### Testleri Çalıştırma

```bash
dotnet test ClinicApp.sln
```

```
Başarılı! - Başarısız: 0, Başarılı: 40, Toplam: 40
├── ClinicApp.UnitTests       → 29 test
└── ClinicApp.IntegrationTests → 11 test
```

**Unit testler (29):**
- `AppointmentTests` — Tüm durum geçişleri ve iş kuralları
- `EmailTests` — Geçerli / geçersiz email senaryoları
- `BookAppointmentHandlerTests` — Handler mantığı (Moq ile sahte DB)
- `CreateDoctorHandlerTests` — Duplicate email kontrolü

**Integration testler (11):**
- `DoctorRepositoryTests` — Gerçek SQLite DB üzerinde CRUD
- `AppointmentRepositoryTests` — Navigation property yüklemesi, durum güncelleme

---

### Örnek İstekler

**Doktor oluştur:**
```json
POST /api/doctors
{
  "firstName": "Ahmet",
  "lastName": "Yılmaz",
  "specialization": "Kardiyoloji",
  "email": "ahmet.yilmaz@klinik.com"
}
```

**Randevu al:**
```json
POST /api/appointments
{
  "patientId": 1,
  "doctorId": 1,
  "scheduledAt": "2026-07-01T10:00:00",
  "notes": "İlk muayene"
}
```

---

---

<a name="en-patient-appointment-system"></a>

## EN — Patient Appointment System

An **ASP.NET Core Web API** for managing doctors, patients, and appointments.  
Goal: hands-on practice of **Clean Architecture**, **CQRS**, **MediatR**, and **Domain-Driven Design** in a real-world scenario.

---

### Architecture

```
ClinicApp/
├── src/
│   ├── ClinicApp.Domain          # Business rules, entities, value objects
│   ├── ClinicApp.Application     # CQRS handlers, pipeline behaviors, validators
│   ClinicApp.Infrastructure  # EF Core, repository implementations, UnitOfWork
│   └── ClinicApp.Api             # Controllers, middleware, Program.cs
└── tests/
    ├── ClinicApp.UnitTests        # Domain and handler unit tests (Moq)
    └── ClinicApp.IntegrationTests # Repository integration tests (SQLite)
```

**Dependency direction:** `Api → Application → Domain` ← `Infrastructure`  
Each layer only knows the layer inside it. Domain has zero dependencies.

---

### Technologies & Concepts

| Layer | Technology / Concept |
|-------|---------------------|
| Domain | Rich Domain Model, Value Objects (`Email`, `PhoneNumber`), DomainException, DataCorruptionException |
| Application | CQRS, MediatR 12, Result Pattern, FluentValidation, Pipeline Behaviors |
| Infrastructure | EF Core 8, SQLite, Fluent API configuration, Value Converters, Repository Pattern, Unit of Work |
| Api | ASP.NET Core 8, Global Exception Middleware, Swagger, Serilog |
| Test | xUnit, Moq, FluentAssertions, SQLite In-Memory |

---

### Domain Layer

#### Entities

**`Patient`** — First name, last name, national ID (11 digits), email, phone, date of birth  
**`Doctor`** — First name, last name, specialization, email. `IsAvailableAt(DateTime)` checks availability at a given time  
**`Appointment`** — Patient + doctor + date + status. Status transitions enforce business rules:

```
Pending → Confirmed → Completed
Pending → Cancelled
Confirmed → Cancelled  (cannot cancel if less than 24 hours away)
Completed → ❌  (cannot cancel or revert)
```

#### Value Objects

`Email.Create()` → user input path, throws `DomainException` if invalid  
`Email.FromDatabase()` → DB read path, throws `DataCorruptionException` if invalid  

This separation distinguishes user errors from infrastructure failures.

---

### CQRS Structure

```
Query   = Read-only, no side effects, returns data
Command = Write operation, contains business rules, mutates the DB
```

Each feature lives in its own folder:

```
Application/
├── Doctors/
│   ├── DoctorDto.cs
│   ├── Queries/
│   │   ├── GetAllDoctors/       → GetAllDoctorsQuery + Handler
│   │   └── GetDoctorById/       → GetDoctorByIdQuery + Handler
│   └── Commands/
│       └── CreateDoctor/        → CreateDoctorCommand + Handler + Validator
├── Patients/  (same structure)
└── Appointments/
    ├── Queries/  (GetAll, GetById)
    └── Commands/ (BookAppointment, ConfirmAppointment, CancelAppointment)
```

---

### Pipeline Behaviors

MediatR middleware layers that intercept every request before the handler runs:

```
Request → LoggingBehavior → ValidationBehavior → Handler → Response
```

**`LoggingBehavior`** — Logs each request name and elapsed time  
**`ValidationBehavior`** — Runs FluentValidation. If invalid, returns `Result.Fail` without ever reaching the handler

---

### Result Pattern

Instead of throwing exceptions, outcomes are carried via `Result<T>`:

```csharp
// In a handler:
if (existing is not null)
    return Error.Conflict("This email is already registered.");

// In a controller:
var result = await _mediator.Send(command);
return result.ToActionResult(); // automatically picks the correct HTTP status
```

| ErrorType | HTTP Status |
|-----------|-------------|
| NotFound | 404 |
| Conflict | 409 |
| Validation | 400 |
| Unexpected | 500 |

---

### API Endpoints

#### Patients

| Method | URL | Description |
|--------|-----|-------------|
| `GET` | `/api/patients` | List all patients |
| `GET` | `/api/patients/{id}` | Get patient by ID |
| `POST` | `/api/patients` | Create a new patient |

#### Doctors

| Method | URL | Description |
|--------|-----|-------------|
| `GET` | `/api/doctors` | List all doctors |
| `GET` | `/api/doctors/{id}` | Get doctor by ID |
| `POST` | `/api/doctors` | Create a new doctor |

#### Appointments

| Method | URL | Description |
|--------|-----|-------------|
| `GET` | `/api/appointments` | List all appointments |
| `GET` | `/api/appointments/{id}` | Get appointment by ID |
| `POST` | `/api/appointments` | Book a new appointment |
| `PUT` | `/api/appointments/{id}/confirm` | Confirm an appointment |
| `PUT` | `/api/appointments/{id}/cancel` | Cancel an appointment |

---

### Getting Started

**Requirements:** .NET 8 SDK

```bash
# Clone the repo
git clone https://github.com/MASalmanss/ClinicApp.git
cd ClinicApp

# Restore dependencies
dotnet restore

# Run the API (migrations are applied automatically on startup)
dotnet run --project src/ClinicApp.Api --launch-profile http
```

Swagger UI: [http://localhost:5130/swagger](http://localhost:5130/swagger)

---

### Running Tests

```bash
dotnet test ClinicApp.sln
```

```
Passed! - Failed: 0, Passed: 40, Total: 40
├── ClinicApp.UnitTests        → 29 tests
└── ClinicApp.IntegrationTests → 11 tests
```

**Unit tests (29):**
- `AppointmentTests` — All status transitions and business rules
- `EmailTests` — Valid / invalid email scenarios
- `BookAppointmentHandlerTests` — Handler logic with Moq fake DB
- `CreateDoctorHandlerTests` — Duplicate email enforcement

**Integration tests (11):**
- `DoctorRepositoryTests` — CRUD against a real SQLite database
- `AppointmentRepositoryTests` — Navigation property loading, status persistence

---

### Sample Requests

**Create a doctor:**
```json
POST /api/doctors
{
  "firstName": "Ahmet",
  "lastName": "Yilmaz",
  "specialization": "Cardiology",
  "email": "ahmet.yilmaz@clinic.com"
}
```

**Book an appointment:**
```json
POST /api/appointments
{
  "patientId": 1,
  "doctorId": 1,
  "scheduledAt": "2026-07-01T10:00:00",
  "notes": "First visit"
}
```

---

### Key Design Decisions

**Why CQRS?**  
Read and write workloads have different concerns. Keeping them separate makes each side simpler, testable independently, and easier to evolve without breaking the other.

**Why Result Pattern instead of exceptions?**  
Exceptions are for truly unexpected situations. Business rule violations (duplicate email, past appointment date) are expected outcomes — they should flow through the normal return path, not the exception path. This makes control flow explicit and forces callers to handle errors.

**Why Value Objects for Email and Phone?**  
A string can hold any value; an `Email` object can only hold a valid email. The type system enforces the constraint, so no validation code is needed anywhere else. The `Create` vs `FromDatabase` distinction separates user-facing validation from data integrity checks.

**Why Fluent API over Data Annotations?**  
Data Annotations put infrastructure knowledge (column length, nullability) inside the domain entity. Fluent API keeps that knowledge in the Infrastructure layer, where it belongs.
