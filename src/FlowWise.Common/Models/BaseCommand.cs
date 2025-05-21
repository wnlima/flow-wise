using MediatR;

namespace FlowWise.Common.Models
{
    /// <summary>
    /// Classe base para todos os comandos MediatR na aplicação Flow Wise.
    /// Inclui propriedades comuns como CorrelationId para rastreabilidade.
    /// </summary>
    /// <typeparam name="TResponse">O tipo da resposta esperada para o comando.</typeparam>
    public abstract class BaseCommand<TResponse> : IRequest<TResponse>
    {
        /// <summary>
        /// Um identificador único para rastrear a execução da requisição através de diferentes serviços.
        /// Gerado automaticamente se não for fornecido.
        /// </summary>
        public string CorrelationId { get; init; }

        public BaseCommand(string correlationId)
        {
            CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId), "CorrelationId não pode ser nulo ou vazio.");
        }
    }

    /// <summary>
    /// Versão de BaseCommand sem retorno específico (para comandos que não precisam de uma resposta).
    /// </summary>
    public abstract class BaseCommand : IRequest
    {
        /// <summary>
        /// Um identificador único para rastrear a execução da requisição através de diferentes serviços.
        /// Gerado automaticamente se não for fornecido.
        /// </summary>
        public string CorrelationId { get; init; }

        public BaseCommand(string correlationId)
        {
            CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId), "CorrelationId não pode ser nulo ou vazio.");
        }
    }
}