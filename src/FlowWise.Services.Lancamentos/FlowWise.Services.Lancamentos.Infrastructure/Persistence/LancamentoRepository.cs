using FlowWise.Services.Lancamentos.Domain.Entities;
using FlowWise.Services.Lancamentos.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlowWise.Services.Lancamentos.Infrastructure.Persistence;

/// <summary>
/// Implementação concreta do repositório de lançamentos, interagindo com o <see cref="LancamentosDbContext"/>.
/// </summary>
public class LancamentoRepository : ILancamentoRepository
{
    private readonly LancamentosDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="LancamentoRepository"/>.
    /// </summary>
    /// <param name="context">O contexto do banco de dados de lançamentos.</param>
    public LancamentoRepository(LancamentosDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona um novo lançamento ao repositório.
    /// </summary>
    /// <param name="lancamento">A entidade de lançamento a ser adicionada.</param>
    /// <returns>Uma tarefa assíncrona.</returns>
    public async Task AddAsync(Lancamento lancamento)
    {
        await _context.Lancamentos.AddAsync(lancamento);
    }

    /// <summary>
    /// Busca um lançamento pelo seu identificador único.
    /// </summary>
    /// <param name="id">O ID do lançamento.</param>
    /// <returns>O lançamento encontrado, ou null se não existir.</returns>
    public async Task<Lancamento?> GetByIdAsync(Guid id)
    {
        // O filtro de query global (HasQueryFilter) em LancamentosDbContext
        // já garante que lançamentos com IsDeleted=true não sejam retornados por padrão.
        return await _context.Lancamentos.FindAsync(id);
    }

    /// <summary>
    /// Retorna todos os lançamentos, com opção de filtros por data e tipo.
    /// </summary>
    /// <param name="startDate">Data de início para o filtro (opcional).</param>
    /// <param name="endDate">Data de fim para o filtro (opcional).</param>
    /// <param name="tipo">Tipo de lançamento para o filtro (opcional: "Credito" ou "Debito").</param>
    /// <param name="categoria">Categoria do lançamento para o filtro (opcional).</param>
    /// <returns>Uma coleção de lançamentos.</returns>
    public async Task<IEnumerable<Lancamento>> GetAllAsync(DateTime? startDate, DateTime? endDate, string? tipo, string? categoria)
    {
        IQueryable<Lancamento> query = _context.Lancamentos;

        if (startDate.HasValue)
        {
            query = query.Where(l => l.Data >= startDate.Value.Date);
        }

        if (endDate.HasValue)
        {
            query = query.Where(l => l.Data <= endDate.Value.Date);
        }

        if (!string.IsNullOrEmpty(tipo))
        {
            query = query.Where(l => l.Tipo.Valor.ToLower() == tipo.ToLower());
        }

        if (!string.IsNullOrEmpty(categoria))
        {
            query = query.Where(l => l.Categoria.Valor.ToLower() == categoria.ToLower());
        }

        return await query.OrderByDescending(l => l.Data).ThenByDescending(l => l.DataCriacao).ToListAsync();
    }

    /// <summary>
    /// Atualiza um lançamento existente no repositório.
    /// </summary>
    /// <param name="lancamento">A entidade de lançamento a ser atualizada.</param>
    /// <returns>Uma tarefa assíncrona.</returns>
    public Task UpdateAsync(Lancamento lancamento)
    {
        _context.Entry(lancamento).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Remove (logicamente) um lançamento do repositório.
    /// A entidade <see cref="Lancamento"/> trata a lógica de marcar IsDeleted = true
    /// e disparar o evento de domínio. O repositório apenas persiste essa mudança.
    /// </summary>
    /// <param name="id">O ID do lançamento a ser removido.</param>
    /// <returns>Uma Task que representa a operação assíncrona.</returns>
    public async Task DeleteAsync(Guid id)
    {
        var lancamento = await GetByIdAsync(id);
        if (lancamento != null)
        {
            _context.Entry(lancamento).State = EntityState.Modified;
        }
    }

    /// <summary>
    /// Salva todas as alterações pendentes no contexto do banco de dados.
    /// Este método efetivamente "comita" a unidade de trabalho.
    /// </summary>
    /// <returns>O número de estados de entidade gravados no banco de dados.</returns>
    public async Task<int> UnitOfWork()
    {
        return await _context.SaveChangesAsync();
    }
}