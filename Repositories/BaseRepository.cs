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
    /// Executes the given operation within a new transaction. If the operation succeeds,
    /// the transaction is committed; otherwise it is rolled back.
    /// Use this to combine multiple repository operations in a single transaction.
    /// </summary>
    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation)
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
    /// Executes the given operation within a new transaction. If the operation succeeds,
    /// the transaction is committed; otherwise it is rolled back.
    /// Use this to combine multiple repository operations in a single transaction.
    /// </summary>
    public async Task ExecuteInTransactionAsync(Func<Task> operation)
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

    /// <summary>
    /// Executes the given operation using an existing transaction.
    /// If no transaction is provided, creates a new one.
    /// Use this to combine multiple repository operations in a single transaction
    /// by passing the same transaction across calls.
    /// </summary>
    public async Task<T> ExecuteWithTransactionAsync<T>(
        IDbContextTransaction? transaction,
        Func<Task<T>> operation
    )
    {
        if (transaction is not null)
        {
            return await operation();
        }

        return await ExecuteInTransactionAsync(operation);
    }

    /// <summary>
    /// Executes the given operation using an existing transaction.
    /// If no transaction is provided, creates a new one.
    /// Use this to combine multiple repository operations in a single transaction
    /// by passing the same transaction across calls.
    /// </summary>
    public async Task ExecuteWithTransactionAsync(
        IDbContextTransaction? transaction,
        Func<Task> operation
    )
    {
        if (transaction is not null)
        {
            await operation();
            return;
        }

        await ExecuteInTransactionAsync(operation);
    }
}
