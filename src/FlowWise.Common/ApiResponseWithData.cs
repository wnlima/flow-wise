namespace FlowWise.Common
{
    /// <summary>
    /// Uma classe de resposta de API que inclui dados específicos da operação.
    /// </summary>
    /// <typeparam name="TData">O tipo dos dados a serem retornados na resposta.</typeparam>
    public class ApiResponseWithData<TData> : ApiResponse
    {
        /// <summary>
        /// Os dados resultantes da operação da API.
        /// </summary>
        public TData? Data { get; private set; }

        private ApiResponseWithData(TData? data, bool success, string? message, IEnumerable<ValidationErrorDetail>? errors, string? correlationId)
            : base(success, message, errors, correlationId)
        {
            Data = data;
        }

        /// <summary>
        /// Cria uma resposta de sucesso com dados.
        /// </summary>
        /// <param name="data">Os dados a serem retornados.</param>
        /// <param name="message">Mensagem de sucesso (opcional).</param>
        /// <param name="correlationId">Identificador de correlação (opcional).</param>
        /// <returns>Uma instância de ApiResponseWithData com os dados e indicando sucesso.</returns>
        public static ApiResponseWithData<TData> Success(TData data, string? message = null, string? correlationId = null)
        {
            return new ApiResponseWithData<TData>(data, true, message ?? "Operação realizada com sucesso.", null, correlationId);
        }

        /// <summary>
        /// Cria uma resposta de erro que poderia, teoricamente, ter dados (embora incomum para erros).
        /// Geralmente, para erros sem dados, usa-se ApiResponse.Error.
        /// Este método existe para consistência, mas seu uso para erros deve ser criterioso.
        /// </summary>
        /// <param name="message">Mensagem de erro principal.</param>
        /// <param name="statusCode">Código de status HTTP (informativo).</param>
        /// <param name="errors">Lista de erros de validação (opcional).</param>
        /// <param name="data">Dados (opcional, raramente usado em erros).</param>
        /// <param name="correlationId">Identificador de correlação (opcional).</param>
        /// <returns>Uma instância de ApiResponseWithData indicando erro.</returns>
        public static new ApiResponseWithData<TData> Error(string message, int? statusCode = null, IEnumerable<ValidationErrorDetail>? errors = null, TData? data = default, string? correlationId = null)
        {
            return new ApiResponseWithData<TData>(data, false, message, errors, correlationId);
        }
    }
}