using FluentValidation;
using MediatR;
using ValidationException = FlowWise.Common.Exceptions.ValidationException; // Precisaremos de uma exceção customizada para validação

namespace FlowWise.Common.Behaviors
{
    /// <summary>
    /// Comportamento de pipeline do MediatR que realiza a validação de requisições
    /// (comandos/queries) usando FluentValidation antes de passá-las para o handler.
    /// </summary>
    /// <typeparam name="TRequest">O tipo da requisição MediatR.</typeparam>
    /// <typeparam name="TResponse">O tipo da resposta da requisição MediatR.</typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="ValidationBehavior{TRequest, TResponse}"/>.
        /// </summary>
        /// <param name="validators">Uma coleção de validadores para a requisição.</param>
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        /// <summary>
        /// Processa a requisição, realizando validações antes de prosseguir com o handler.
        /// </summary>
        /// <param name="request">A requisição a ser validada.</param>
        /// <param name="next">O próximo delegate no pipeline do MediatR.</param>
        /// <param name="cancellationToken">O token de cancelamento.</param>
        /// <returns>A resposta da requisição.</returns>
        /// <exception cref="ValidationException">Lançada se alguma validação falhar.</exception>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    throw new ValidationException(failures);
                }
            }
            return await next();
        }
    }
}