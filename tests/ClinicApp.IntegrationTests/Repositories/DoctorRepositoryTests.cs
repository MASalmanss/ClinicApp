using ClinicApp.Domain.Entities;
using ClinicApp.Domain.ValueObjects;
using ClinicApp.Infrastructure.Persistence;
using ClinicApp.Infrastructure.Persistence.Repositories;
using FluentAssertions;

namespace ClinicApp.IntegrationTests.Repositories;

public class DoctorRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly DoctorRepository _repository;

    public DoctorRepositoryTests()
    {
        _context = TestDbContextFactory.Create();
        _repository = new DoctorRepository(_context);
    }

    public void Dispose() => TestDbContextFactory.Destroy(_context);

    // ─── Yardımcı ───
    private static Doctor MakeDoctor(string email = "ahmet@klinik.com")
        => Doctor.Create("Ahmet", "Yılmaz", "Kardiyoloji", Email.Create(email));

    // ─── Testler ───

    [Fact]
    public async Task AddAsync_ThenGetById_ReturnsSameDoctor()
    {
        var doctor = MakeDoctor();
        await _repository.AddAsync(doctor);
        await _context.SaveChangesAsync();

        var found = await _repository.GetByIdAsync(doctor.Id);

        found.Should().NotBeNull();
        found!.FullName.Should().Be("Dr. Ahmet Yılmaz");
        found.Email.Value.Should().Be("ahmet@klinik.com");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllDoctors()
    {
        await _repository.AddAsync(MakeDoctor("a@klinik.com"));
        await _repository.AddAsync(MakeDoctor("b@klinik.com"));
        await _context.SaveChangesAsync();

        var doctors = await _repository.GetAllAsync();

        doctors.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByEmailAsync_ExistingEmail_ReturnsDoctor()
    {
        await _repository.AddAsync(MakeDoctor("ahmet@klinik.com"));
        await _context.SaveChangesAsync();

        var found = await _repository.GetByEmailAsync("ahmet@klinik.com");

        found.Should().NotBeNull();
        found!.Specialization.Should().Be("Kardiyoloji");
    }

    [Fact]
    public async Task GetByEmailAsync_NonExistentEmail_ReturnsNull()
    {
        var found = await _repository.GetByEmailAsync("yok@yok.com");

        found.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        var found = await _repository.GetByIdAsync(999);

        found.Should().BeNull();
    }

    [Fact]
    public async Task Delete_RemovesDoctorFromDb()
    {
        var doctor = MakeDoctor();
        await _repository.AddAsync(doctor);
        await _context.SaveChangesAsync();

        _repository.Delete(doctor);
        await _context.SaveChangesAsync();

        var found = await _repository.GetByIdAsync(doctor.Id);
        found.Should().BeNull();
    }
}
