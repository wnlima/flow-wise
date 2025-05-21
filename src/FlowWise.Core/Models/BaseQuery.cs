namespace FlowWise.Common.Models
{
    /// <summary>
    /// Classe base para todas as Queries no sistema.
    /// </summary>
    /// <remarks>
    /// NFR-OBS-001: O sistema deve registrar um Correlation ID para rastreabilidade de requisições.
    /// </remarks>
    public abstract class BaseQuery
    {
        /// <summary>
        /// O identificador de correlação para rastrear a requisição através do sistema.
        /// </summary>
        public string CorrelationId { get; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="BaseQuery"/>.
        /// </summary>
        /// <param name="correlationId">O identificador de correlação para rastreabilidade.
        /// Se nulo ou vazio, um novo GUID será gerado.</param>
        protected BaseQuery(string? correlationId)
        {
            CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid().ToString() : correlationId;
        }
    }
}