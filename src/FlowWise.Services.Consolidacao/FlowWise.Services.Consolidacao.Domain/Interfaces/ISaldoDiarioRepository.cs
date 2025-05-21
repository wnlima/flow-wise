using FlowWise.Services.Consolidacao.Domain.Entities;

namespace FlowWise.Services.Consolidacao.Domain.Interfaces
{
    /// <summary>
    /// [DDD] Abstração: Interface do repositório para a entidade <see cref="SaldoDiario"/>.
    /// Define o contrato para operações de persistência (adicionar, buscar, atualizar)
    /// de objetos do tipo <see cref="SaldoDiario"/>.
    /// </summary>
    public interface ISaldoDiarioRepository
    {
        /// <summary>
        /// Adiciona um novo registro de saldo diário ao repositório.
        /// </summary>
        /// <param name="saldoDiario">O objeto SaldoDiario a ser adicionado.</param>
        /// <returns>Uma Task que representa a operação assíncrona.</returns>
        Task AddAsync(SaldoDiario saldoDiario);

        /// <summary>
        /// Obtém um registro de saldo diário pela sua data.
        /// </summary>
        /// <param name="data">A data do saldo diário.</param>
        /// <returns>Uma Task que representa a operação assíncrona, retornando o SaldoDiario ou null se não encontrado.</returns>
        Task<SaldoDiario?> GetByDateAsync(DateTime data);

        /// <summary>
        /// Atualiza um registro de saldo diário existente no repositório.
        /// </summary>
        /// <param name="saldoDiario">O objeto SaldoDiario a ser atualizado.</param>
        /// <returns>Uma Task que representa a operação assíncrona.</returns>
        Task UpdateAsync(SaldoDiario saldoDiario);

        /// <summary>
        /// Obtém um intervalo de registros de saldo diário.
        /// </summary>
        /// <param name="dataInicio">A data de início do intervalo (inclusiva).</param>
        /// <param name="dataFim">A data de fim do intervalo (inclusiva).</param>
        /// <returns>Uma Task que representa a operação assíncrona, retornando uma coleção de SaldoDiario.</returns>
        Task<IEnumerable<SaldoDiario>> GetByDateRangeAsync(DateTime dataInicio, DateTime dataFim);

        /// <summary>
        /// Persiste todas as mudanças pendentes na unidade de trabalho.
        /// </summary>
        /// <returns>Uma Task que representa a operação assíncrona, retornando o número de entradas de estado escritas com sucesso.</returns>
        Task<int> UnitOfWork();
    }
}