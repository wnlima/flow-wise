
namespace FlowWise.Common
{
    /// <summary>
    /// Classe base para padronizar as respostas de API.
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Indica se a operação da API foi bem-sucedida.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Uma mensagem geral relacionada à operação.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Uma coleção de detalhes de erros de validação, se houver.
        /// </summary>
        public IEnumerable<ValidationErrorDetail>? Errors { get; set; }

        /// <summary>
        /// O identificador de correlação para rastreabilidade.
        /// </summary>
        public string? CorrelationId { get; set; }

        protected ApiResponse(bool success, string? message, IEnumerable<ValidationErrorDetail>? errors, string? correlationId)
        {
            Success = success;
            Message = message;
            Errors = errors;
            CorrelationId = correlationId;
        }

        /// <summary>
        /// Cria uma resposta de sucesso.
        /// </summary>
        /// <param name="message">Mensagem de sucesso (opcional).</param>
        /// <param name="correlationId">Identificador de correlação (opcional).</param>
        /// <returns>Uma instância de ApiResponse indicando sucesso.</returns>
        public static ApiResponse SuccessResult(string? message = null, string? correlationId = null)
        {
            return new ApiResponse(true, message ?? "Operação realizada com sucesso.", null, correlationId);
        }

        /// <summary>
        /// Cria uma resposta de erro.
        /// </summary>
        /// <param name="message">Mensagem de erro principal.</param>
        /// <param name="statusCode">Código de status HTTP (informativo, não define o status da resposta HTTP real aqui).</param>
        /// <param name="errors">Lista de erros de validação (opcional).</param>
        /// <param name="correlationId">Identificador de correlação (opcional).</param>
        /// <returns>Uma instância de ApiResponse indicando erro.</returns>
        public static ApiResponse ErrorResult(string message, int? statusCode = null, IEnumerable<ValidationErrorDetail>? errors = null, string? correlationId = null)
        {
            return new ApiResponse(false, message, errors, correlationId);
        }
    }
}