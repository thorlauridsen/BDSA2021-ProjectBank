
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProjectBank.Core;
using ProjectBank.Infrastructure;
using Xunit;
using static ProjectBank.Core.Status;

namespace Infrastructure.Tests;

public class StudentRepositoryTests : IDisposable
{
    private readonly ProjectBankContext _context;
    private readonly StudentRepository _repository;

    public StudentRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ProjectBankContext>();
        builder.UseSqlite(connection);
        var context = new ProjectBankContext(builder.Options);
        context.Database.EnsureCreated();

        var user = new Student { Id = 1, Name = "Claus" };
        context.Students.Add(user);
        context.SaveChanges();

        _context = context;
        _repository = new StudentRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_creates_new_user_with_generated_id()
    {
        var user = new StudentCreateDto { Name = "Karsten" };
        var created = await _repository.CreateAsync(user);

        Assert.Equal(2, created.Id);
        Assert.Equal(user.Name, created.Name);

        var users = await _repository.ReadAsync();

        Assert.Collection(users,
            s => Assert.Equal(new StudentDto(1, "Claus"), s),
            s => Assert.Equal(new StudentDto(2, "Karsten"), s)
        );
    }

    [Fact]
    public async Task ReadAsync_returns_all_users()
    {
        var users = await _repository.ReadAsync();

        Assert.Collection(users,
            s => Assert.Equal(new StudentDto(1, "Claus"), s)
        );
    }

    [Fact]
    public async Task ReadAsync_given_non_existing_id_returns_None()
    {
        var option = await _repository.ReadAsync(11);

        Assert.True(option.IsNone);
    }

    [Fact]
    public async Task DeleteAsync_given_non_existing_Id_returns_NotFound()
    {
        var response = await _repository.DeleteAsync(11);

        Assert.Equal(NotFound, response);
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
