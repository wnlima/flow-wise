using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using FlowWise.Common.Models;

namespace FlowWise.Common.Behaviors
{
    /// <summary>
    /// Comportamento de pipeline do MediatR que adiciona um Correlation ID a todas as requisições (comandos/queries).
    /// Isso é essencial para rastreabilidade de requisições em sistemas distribuídos e para observabilidade.
    /// </summary>
    /// <typeparam name="TRequest">O tipo da requisição MediatR.</typeparam>
    /// <typeparam name="TResponse">O tipo da resposta da requisição MediatR.</typeparam>
    public class CorrelationIdBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<CorrelationIdBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="CorrelationIdBehavior{TRequest, TResponse}"/>.
        /// </summary>
        /// <param name="logger">O logger para registrar informações.</param>
        public CorrelationIdBehavior(ILogger<CorrelationIdBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Processa a requisição adicionando um Correlation ID ao contexto de log.
        /// </summary>
        /// <param name="request">A requisição a ser processada.</param>
        /// <param name="next">O próximo delegate no pipeline do MediatR.</param>
        /// <param name="cancellationToken">O token de cancelamento.</param>
        /// <returns>A resposta da requisição.</returns>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            string correlationId;
            var requestAsBaseCommand = request as BaseCommand;
            var requestAsBaseQuery = request as BaseQuery<TResponse>;

            if (requestAsBaseCommand != null && !string.IsNullOrEmpty(requestAsBaseCommand.CorrelationId))
            {
                correlationId = requestAsBaseCommand.CorrelationId;
            }
            else if (requestAsBaseQuery != null && !string.IsNullOrEmpty(requestAsBaseQuery.CorrelationId))
            {
                correlationId = requestAsBaseQuery.CorrelationId;
            }
            else
            {
                correlationId = Guid.NewGuid().ToString();
            }

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                _logger.LogInformation("Processing request {RequestName} with CorrelationId: {CorrelationId}", typeof(TRequest).Name, correlationId);
                return await next();
            }
        }
    }
}