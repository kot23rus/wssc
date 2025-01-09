using Microsoft.EntityFrameworkCore;

namespace WSSC.Data;

/// <summary>
/// Контекст подключения к БД
/// </summary>
public class CompanyDbContext(DbContextOptions<CompanyDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Таблица для хранения объектов данных
    /// </summary>
    public DbSet<Entity> Companies { get; set; }

}
