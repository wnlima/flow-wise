using FlowWise.Services.Consolidacao.Domain.Entities;
using FlowWise.Services.Consolidacao.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlowWise.Services.Consolidacao.Infrastructure.Persistence;

/// <summary>
/// Implementação concreta do repositório para a entidade <see cref="SaldoDiario"/>.
/// Interage com o <see cref="ConsolidacaoDbContext"/> para operações de persistência.
/// </summary>
/// <remarks>
/// [ADR-PERSISTENCE-001]: Decisão de usar Entity Framework Core para persistência.
/// [DDD]: Implementa o contrato definido na interface de repositório do domínio.
/// </remarks>
public class SaldoDiarioRepository : ISaldoDiarioRepository
{
    private readonly ConsolidacaoDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="SaldoDiarioRepository"/>.
    /// </summary>
    /// <param name="context">O contexto do banco de dados de consolidação.</param>
    public SaldoDiarioRepository(ConsolidacaoDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona um novo registro de saldo diário ao repositório.
    /// </summary>
    /// <param name="saldoDiario">O objeto SaldoDiario a ser adicionado.</param>
    /// <returns>Uma Task que representa a operação assíncrona.</returns>
    public async Task AddAsync(SaldoDiario saldoDiario)
    {
        await _context.SaldosDiarios.AddAsync(saldoDiario);
    }

    /// <summary>
    /// Obtém um registro de saldo diário pela sua data.
    /// </summary>
    /// <param name="data">A data do saldo diário.</param>
    /// <returns>Uma Task que representa a operação assíncrona, retornando o SaldoDiario ou null se não encontrado.</returns>
    public async Task<SaldoDiario?> GetByDateAsync(DateTime data)
    {
        return await _context.SaldosDiarios.FirstOrDefaultAsync(sd => sd.Data == data.Date);
    }

    /// <summary>
    /// Atualiza um registro de saldo diário existente no repositório.
    /// No EF Core, se a entidade já está sendo rastreada e suas propriedades alteradas,
    /// basta chamar SaveChangesAsync. Se for uma entidade desanexada, Attach e MarkAsModified.
    /// </summary>
    /// <param name="saldoDiario">O objeto SaldoDiario a ser atualizado.</param>
    public Task UpdateAsync(SaldoDiario saldoDiario)
    {
        _context.Entry(saldoDiario).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Obtém um intervalo de registros de saldo diário.
    /// </summary>
    /// <param name="dataInicio">A data de início do intervalo (inclusiva).</param>
    /// <param name="dataFim">A data de fim do intervalo (inclusiva).</param>
    /// <returns>Uma Task que representa a operação assíncrona, retornando uma coleção de SaldoDiario.</returns>
    public async Task<IEnumerable<SaldoDiario>> GetByDateRangeAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _context.SaldosDiarios
                             .AsNoTracking()
                             .Where(sd => sd.Data >= dataInicio.Date && sd.Data <= dataFim.Date)
                             .OrderBy(sd => sd.Data)
                             .ToListAsync();
    }

    /// <summary>
    /// Persiste todas as mudanças pendentes na unidade de trabalho.
    /// </summary>
    /// <returns>Uma Task que representa a operação assíncrona, retornando o número de entradas de estado escritas com sucesso.</returns>
    public async Task<int> UnitOfWork()
    {
        return await _context.SaveChangesAsync();
    }
}
