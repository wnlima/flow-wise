namespace FlowWise.Services.Consolidacao.Domain.Exceptions
{
    /// <summary>
    /// Exceção personalizada para erros que ocorrem dentro da camada de domínio de Consolidação.
    /// Sinaliza que uma regra de negócio ou invariante de domínio foi violada.
    /// </summary>
    public class ConsolidacaoDomainException : Exception
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ConsolidacaoDomainException"/>.
        /// </summary>
        public ConsolidacaoDomainException() { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ConsolidacaoDomainException"/> com uma mensagem de erro especificada.
        /// </summary>
        /// <param name="message">A mensagem que descreve o erro.</param>
        public ConsolidacaoDomainException(string message)
            : base(message) { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ConsolidacaoDomainException"/> com uma mensagem de erro especificada
        /// e uma referência à exceção interna que é a causa desta exceção.
        /// </summary>
        /// <param name="message">A mensagem de erro que explica a razão da exceção.</param>
        /// <param name="innerException">A exceção que é a causa da exceção atual, ou uma referência nula se nenhuma exceção interna for especificada.</param>
        public ConsolidacaoDomainException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}