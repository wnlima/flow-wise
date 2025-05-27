using FlowWise.Common;
using FlowWise.Common.Exceptions;
using FlowWise.Services.Consolidacao.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace FlowWise.Services.Consolidacao.Api.Middleware
{
    /// <summary>
    /// Middleware para tratamento centralizado de exceções na pipeline de requisições HTTP.
    /// Captura exceções conhecidas e não tratadas, loga-as e formata uma resposta padronizada
    /// do tipo <see cref="ApiResponse"/> para o cliente.
    /// </summary>
    /// <remarks>
    /// Este middleware contribui para:
    /// - NFR-SEC-003: Proteção contra divulgação de informações sensíveis em erros (OWASP Top 10 - Security Misconfiguration/Sensitive Data Exposure).
    /// - NFR-USG-001: Fornecer feedback claro e consistente ao usuário em caso de erros.
    /// - NFR-OBS-001: Garante que o Correlation ID seja incluído nas respostas de erro para rastreabilidade.
    /// </remarks>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ErrorHandlingMiddleware"/>.
        /// </summary>
        /// <param name="next">O próximo delegado na pipeline de requisições.</param>
        /// <param name="logger">O logger para registrar informações de exceção.</param>
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invoca o middleware para processar a requisição HTTP.
        /// Tenta executar o próximo middleware na pipeline e captura quaisquer exceções que ocorram.
        /// </summary>
        /// <param name="context">O <see cref="HttpContext"/> para a requisição atual.</param>
        /// <returns>Uma <see cref="Task"/> que representa a conclusão do processamento da requisição.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Tenta obter o Correlation ID do header. Se não existir, gera um novo.
            // Este Correlation ID é adicionado ao context.Items para que possa ser usado por outros
            // componentes da aplicação, como loggers ou outros middlewares, se necessário.
            string correlationId = context.Request.Headers["x-correlation-id"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                // Adicionar ao header da requisição para que possa ser pego pelo CorrelationIdBehavior do MediatR, se necessário.
                // No entanto, o CorrelationIdBehavior já tenta pegar do header ou gerar um novo.
                // O principal aqui é garantir que temos um para a resposta de erro.
            }
            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers["x-correlation-id"] = correlationId;


            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Delega o tratamento da exceção para um método dedicado.
                await HandleExceptionAsync(context, ex, correlationId);
            }
        }

        /// <summary>
        /// Lida com a exceção capturada, loga os detalhes e envia uma resposta de erro padronizada.
        /// </summary>
        /// <param name="context">O <see cref="HttpContext"/>.</param>
        /// <param name="exception">A exceção capturada.</param>
        /// <param name="correlationId">O identificador de correlação associado à requisição.</param>
        /// <returns>Uma <see cref="Task"/> que representa a escrita da resposta de erro.</returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
        {
            context.Response.ContentType = "application/json";
            ApiResponse apiResponse;

            switch (exception)
            {
                case ValidationException validationEx:
                    // Erros de validação conhecidos, provenientes do FluentValidation via ValidationBehavior.
                    // REQ-FLW-COM-001: O sistema deve validar os dados de entrada e retornar mensagens de erro claras.
                    _logger.LogWarning(validationEx,
                        "Erro de validação. CorrelationId: {CorrelationId}. Path: {Path}. Detalhes: {ValidationErrors}",
                        correlationId, context.Request.Path, JsonSerializer.Serialize(validationEx.Errors));
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse = ApiResponse.ErrorResult("Um ou mais erros de validação ocorreram.", context.Response.StatusCode, validationEx.Errors, correlationId);
                    break;

                case ConsolidacaoDomainException domainEx:
                    // Exceções específicas de regras de negócio do domínio de Lançamentos.
                    // Exemplo: RN-001: Lançamentos de dias anteriores não podem ser editados ou excluídos.
                    _logger.LogWarning(domainEx,
                        "Erro de negócio no domínio Lançamentos. CorrelationId: {CorrelationId}. Path: {Path}. Mensagem: {DomainExceptionMessage}",
                        correlationId, context.Request.Path, domainEx.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.UnprocessableContent;
                    apiResponse = ApiResponse.ErrorResult(domainEx.Message, context.Response.StatusCode, correlationId: correlationId);
                    break;

                // TODO: Adicionar aqui o tratamento para outras exceções de domínio específicas de outros serviços, se este middleware for compartilhado.

                default:
                    // NFR-SEC-003: Evitar expor detalhes internos do sistema em mensagens de erro genéricas.
                    _logger.LogError(exception,
                        "Erro inesperado no servidor. CorrelationId: {CorrelationId}. Path: {Path}. Tipo Exceção: {ExceptionType}. Mensagem: {ExceptionMessage}",
                        correlationId, context.Request.Path, exception.GetType().FullName, exception.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    apiResponse = ApiResponse.ErrorResult("Ocorreu um erro inesperado no servidor. Por favor, tente novamente mais tarde ou contate o suporte.", context.Response.StatusCode, correlationId: correlationId);
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}