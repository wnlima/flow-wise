using FlowWise.Services.Lancamentos.Domain.Entities;

namespace FlowWise.Services.Lancamentos.Domain.Interfaces
{
    /// <summary>
    /// [DDD] Abstração: Interface do repositório para o agregado <see cref="Lancamento"/>.
    /// Define o contrato para operações de persistência (adicionar, buscar, atualizar, remover)
    /// de objetos do tipo <see cref="Lancamento"/>.
    /// </summary>
    public interface ILancamentoRepository
    {
        /// <summary>
        /// Adiciona um novo lançamento ao repositório.
        /// </summary>
        /// <param name="lancamento">O objeto Lancamento a ser adicionado.</param>
        /// <returns>Uma Task que representa a operação assíncrona.</returns>
        Task AddAsync(Lancamento lancamento);

        /// <summary>
        /// Obtém um lançamento pelo seu identificador único.
        /// </summary>
        /// <param name="id">O ID do lançamento.</param>
        /// <returns>Uma Task que representa a operação assíncrona, retornando o Lancamento ou null se não encontrado.</returns>
        Task<Lancamento?> GetByIdAsync(Guid id);

        /// <summary>
        /// Retorna todos os lançamentos, com opção de filtros por data e tipo.
        /// </summary>
        /// <param name="startDate">Data de início para o filtro (opcional).</param>
        /// <param name="endDate">Data de fim para o filtro (opcional).</param>
        /// <param name="tipo">Tipo de lançamento para o filtro (opcional: "Credito" ou "Debito").</param>
        /// <param name="categoria">Categoria do lançamento para o filtro (opcional).</param>
        /// <returns>Uma coleção de lançamentos.</returns>
        Task<IEnumerable<Lancamento>> GetAllAsync(DateTime? startDate, DateTime? endDate, string? tipo, string? categoria);

        /// <summary>
        /// Atualiza um lançamento existente no repositório.
        /// </summary>
        /// <param name="lancamento">O objeto Lancamento a ser atualizado.</param>
        /// <returns>Uma Task que representa a operação assíncrona.</returns>
        Task UpdateAsync(Lancamento lancamento);

        /// <summary>
        /// Remove um lançamento do repositório pelo seu identificador único.
        /// </summary>
        /// <param name="id">O ID do lançamento a ser removido.</param>
        /// <returns>Uma Task que representa a operação assíncrona.</returns>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Persiste todas as mudanças pendentes na unidade de trabalho.
        /// </summary>
        /// <returns>Uma Task que representa a operação assíncrona, retornando o número de entradas de estado escritas com sucesso.</returns>
        Task<int> UnitOfWork();
    }
}