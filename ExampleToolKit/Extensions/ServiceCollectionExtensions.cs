using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adiciona o serviço de logging de eventos de domínio usando o contexto especificado
    /// </summary>
    public static IServiceCollection AddDomainEventLogging<TContext>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction = null!)
        where TContext : DbContext
    {
        // Registra o logger usando o contexto principal
        services.AddScoped<IDomainEventLogger, DbDomainEventLogger>(sp =>
            new DbDomainEventLogger(sp.GetRequiredService<TContext>()));

        return services;
    }

    /// <summary>
    /// Adiciona o serviço de logging de eventos de domínio usando um contexto separado
    /// </summary>
    public static IServiceCollection AddDomainEventLoggingWithSeparateContext(
        this IServiceCollection services,
        string connectionString,
        string contextName = "EventLogContext")
    {
        // Registra um contexto separado para logs
        services.AddDbContext<EventLogDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        // Registra o logger usando o contexto separado
        services.AddScoped<IDomainEventLogger, DbDomainEventLogger>(sp =>
            new DbDomainEventLogger(sp.GetRequiredService<EventLogDbContext>()));

        return services;
    }
}

// Classe de contexto específica para logs de eventos
public class EventLogDbContext : DbContext
{
    public EventLogDbContext(DbContextOptions<EventLogDbContext> options) : base(options)
    {
    }

    public DbSet<DomainEventLog> DomainEventLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica a configuração para DomainEventLog
        modelBuilder.ApplyConfiguration(new DomainEventLogConfiguration());
    }
}