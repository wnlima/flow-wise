using MediatR;

namespace FlowWise.Common.Models
{
    /// <summary>
    /// Classe base para todas as queries MediatR na aplicação Flow Wise.
    /// Inclui propriedades comuns como CorrelationId para rastreabilidade.
    /// </summary>
    /// <typeparam name="TResponse">O tipo da resposta esperada para a query.</typeparam>
    public abstract class BaseQuery<TResponse> : IRequest<TResponse>
    {
        /// <summary>
        /// Um identificador único para rastrear a execução da requisição através de diferentes serviços.
        /// Gerado automaticamente se não for fornecido.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="BaseQuery{TResponse}"/>.
        /// </summary>
        /// <param name="correlationId">O identificador de correlação para rastreabilidade. Se nulo ou vazio, um novo GUID será gerado.</param>
        public BaseQuery(string correlationId)
        {
            CorrelationId = correlationId;
            if (String.IsNullOrEmpty(CorrelationId))
            {
                CorrelationId = Guid.NewGuid().ToString();
            }
        }
    }
}