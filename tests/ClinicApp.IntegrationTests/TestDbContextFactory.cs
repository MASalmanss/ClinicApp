using ClinicApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.IntegrationTests;

/// <summary>
/// Her test için izole bir in-memory SQLite DB oluşturur.
/// Testler birbirini etkilemez.
/// </summary>
public static class TestDbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")  // memory DB — disk'e yazmaz
            .Options;

        var context = new AppDbContext(options);

        // Bağlantıyı açık tut (memory DB bağlantı kapanınca silinir)
        context.Database.OpenConnection();
        context.Database.EnsureCreated();  // Migration'lar yerine schema'yı direkt oluşturur

        return context;
    }

    public static void Destroy(AppDbContext context)
    {
        context.Database.CloseConnection();
        context.Dispose();
    }
}
