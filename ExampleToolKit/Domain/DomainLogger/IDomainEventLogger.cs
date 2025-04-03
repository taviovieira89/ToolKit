using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IDomainEventLogger
{
    /// <summary>
    /// Registra um evento de domínio
    /// </summary>
    Task LogEventAsync(object domainEvent, string aggregateId, string aggregateName);

    /// <summary>
    /// Marca um evento como processado
    /// </summary>
    Task MarkEventAsProcessedAsync(object domainEvent);

    /// <summary>
    /// Obtém eventos por ID do agregado
    /// </summary>
    Task<IEnumerable<object>> GetEventsByAggregateIdAsync(string aggregateId, Type eventType = null!);

    /// <summary>
    /// Obtém eventos não processados
    /// </summary>
    Task<IEnumerable<object>> GetUnprocessedEventsAsync(Type eventType = null!);
}

