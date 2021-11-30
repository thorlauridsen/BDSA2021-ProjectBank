using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProjectBank.Core;
using ProjectBank.Infrastructure;
using Xunit;

namespace Infrastructure.Tests;

public class SupervisorRepositoryTests : IDisposable
{
    private readonly ProjectBankContext _context;
    private readonly SupervisorRepository _repository;

    public SupervisorRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ProjectBankContext>();
        builder.UseSqlite(connection);
        var context = new ProjectBankContext(builder.Options);
        context.Database.EnsureCreated();

        var claus = new Supervisor { Id = 1, Name = "Claus" };

        context.Supervisors.AddRangeAsync(
            claus
            );

        context.SaveChanges();

        _context = context;
        _repository = new SupervisorRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_creates_new_supervisor_with_generated_id()
    {
        var supervisor = new SupervisorCreateDto { Name = "Karsten" };

        var created = await _repository.CreateAsync(supervisor);

        Assert.Equal(2, created.Id);
        Assert.Equal(supervisor.Name, created.Name);

        var supervisors = await _repository.ReadAsync();

        Assert.Collection(supervisors,
            s => Assert.Equal(new SupervisorDto(1, "Claus"), s),
            s => Assert.Equal(new SupervisorDto(2, "Karsten"), s)
        );
    }

    [Fact]
    public async Task ReadAsync_returns_all_supervisors()
    {
        var supervisors = await _repository.ReadAsync();

        Assert.Collection(supervisors,
            s => Assert.Equal(new SupervisorDto(1, "Claus"), s)
        );
    }

    private bool disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. 
        // Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
