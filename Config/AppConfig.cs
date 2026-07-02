using System.Data;

namespace MyApp.Config;

/// <summary>
/// Centralized application configuration loaded from environment variables.
/// Can be accessed via DI (ISingleton) or statically via <see cref="Instance"/>.
/// </summary>
public class AppConfig
{
    private static AppConfig? _instance;
    private static readonly object _lock = new();

    /// <summary>
    /// Gets the singleton instance, loading environment variables on first access.
    /// </summary>
    public static AppConfig Instance
    {
        get
        {
            if (_instance is null)
            {
                lock (_lock)
                {
                    if (_instance is null)
                    {
                        DotNetEnv.Env.Load();
                        _instance = new AppConfig();
                    }
                }
            }
            return _instance;
        }
    }

    /// <summary>PostgreSQL connection string.</summary>
    public string DatabaseUrl { get; }

    /// <summary>Transaction isolation level for database operations.</summary>
    public IsolationLevel TransactionIsolationLevel { get; }

    /// <summary>ASP.NET Core environment name (Development, Production, etc.).</summary>
    public string AspnetcoreEnvironment { get; }

    /// <summary>Host address to bind the web server to.</summary>
    public string Host { get; }

    /// <summary>Port number to bind the web server to.</summary>
    public int Port { get; }

    private AppConfig()
    {
        DatabaseUrl = DotNetEnv.Env.GetString("DATABASE_URL");
        AspnetcoreEnvironment = DotNetEnv.Env.GetString("ASPNETCORE_ENVIRONMENT", "Production");

        var isolationLevelStr = DotNetEnv.Env.GetString(
            "TRANSACTION_ISOLATION_LEVEL",
            "ReadCommitted"
        );
        TransactionIsolationLevel = Enum.Parse<IsolationLevel>(isolationLevelStr, ignoreCase: true);

        Host = DotNetEnv.Env.GetString("HOST", "localhost");
        Port = DotNetEnv.Env.GetInt("PORT", 5000);
    }
}
