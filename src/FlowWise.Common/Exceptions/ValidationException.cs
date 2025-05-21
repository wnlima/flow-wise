using FluentValidation.Results;

namespace FlowWise.Common.Exceptions
{
    /// <summary>
    /// Exceção personalizada para erros de validação, encapsulando múltiplos erros.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Obtém uma lista de detalhes de erros de validação.
        /// </summary>
        public List<ValidationErrorDetail> Errors { get; }

        /// <summary>
        /// Construtor padrão da exceção de validação.
        /// </summary>
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new List<ValidationErrorDetail>();
        }

        /// <summary>
        /// Constrói uma exceção de validação a partir de uma lista de falhas do FluentValidation.
        /// </summary>
        /// <param name="failures">A lista de falhas de validação.</param>
        public ValidationException(List<ValidationFailure> failures)
            : this()
        {
            Errors.AddRange(failures.Select(f => new ValidationErrorDetail
            {
                PropertyName = f.PropertyName,
                ErrorMessage = f.ErrorMessage
            }));
        }
    }
}