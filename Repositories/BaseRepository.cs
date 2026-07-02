using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyApp.Config;
using MyApp.Data;

namespace MyApp.Repositories;

public abstract class BaseRepository
{
    protected readonly AppDbContext _db;
    private readonly IsolationLevel _defaultIsolationLevel;

    protected BaseRepository(AppDbContext db)
    {
        _db = db;
        _defaultIsolationLevel = AppConfig.Instance.TransactionIsolationLevel;
    }

    /// <summary>
    /// Begins a database transaction with the configured default isolation level.
    /// </summary>
    protected async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _db.Database.BeginTransactionAsync(_defaultIsolationLevel);
    }

    /// <summary>
    /// Begins a database transaction with a specified isolation level.
    /// </summary>
    protected async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel)
    {
        return await _db.Database.BeginTransactionAsync(isolationLevel);
    }

    /// <summary>
    /// Executes the given operation within a transaction. If the operation succeeds,
    /// the transaction is committed; otherwise it is rolled back.
    /// </summary>
    protected async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation)
    {
        await using var transaction = await BeginTransactionAsync();
        try
        {
            var result = await operation();
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Executes the given operation within a transaction. If the operation succeeds,
    /// the transaction is committed; otherwise it is rolled back.
    /// </summary>
    protected async Task ExecuteInTransactionAsync(Func<Task> operation)
    {
        await using var transaction = await BeginTransactionAsync();
        try
        {
            await operation();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
