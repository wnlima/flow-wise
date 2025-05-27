using FluentValidation;
using FlowWise.Services.Lancamentos.Application.Commands;
using FlowWise.Services.Lancamentos.Domain.ValueObjects;

namespace FlowWise.Services.Lancamentos.Application.Validators
{
    /// <summary>
    /// Validador para o comando <see cref="CreateLancamentoCommand"/> utilizando FluentValidation.
    /// Garante que os dados fornecidos para a criação de um novo lançamento são válidos
    /// antes que a lógica de negócio seja executada.
    /// </summary>
    /// <remarks>
    /// REQ-FLW-COM-001: O sistema deve validar os dados de entrada e retornar mensagens de erro claras.
    /// NFR-SEC-003: Prevenção de ataques de injeção e dados inválidos (OWASP Top 10 - Validação de Entrada).
    /// TDD: Testes unitários (CreateLancamentoCommandValidatorTests) cobrem estas regras de validação.
    /// </remarks>
    public class CreateLancamentoCommandValidator : AbstractValidator<CreateLancamentoCommand>
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="CreateLancamentoCommandValidator"/>.
        /// Define as regras de validação para o comando de criação de lançamento.
        /// </summary>
        public CreateLancamentoCommandValidator()
        {
            // REQ-FLW-LAN-RN-006: O valor do lançamento deve ser maior que zero.
            RuleFor(x => x.Valor)
                .GreaterThan(0).WithMessage("O valor do lançamento deve ser maior que zero. [RN-006]");

            // REQ-FLW-LAN-RN-002: A data do lançamento não pode ser futura.
            RuleFor(x => x.Data)
                .NotEmpty().WithMessage("A data do lançamento é obrigatória.") // Adicionado NotEmpty para Data
                .Must(date => date.Date <= DateTime.Today.Date).WithMessage("A data do lançamento não pode ser futura. [RN-002]");

            // REQ-FLW-LAN-HU-002 (História de Usuário que implica obrigatoriedade e formato)
            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("A descrição do lançamento é obrigatória.")
                .MaximumLength(255).WithMessage("A descrição do lançamento não pode exceder 255 caracteres.");

            // REQ-FLW-LAN-HU-001 (História de Usuário que implica validade do tipo)
            RuleFor(x => x.Tipo)
                .NotEmpty().WithMessage("O tipo de lançamento é obrigatório.")
                .Must(BeValidTipoLancamento).WithMessage($"O tipo de lançamento deve ser '{TipoLancamento.Credito.Valor}' ou '{TipoLancamento.Debito.Valor}'.");

            // REQ-FLW-LAN-HU-002 (História de Usuário que implica validade da categoria)
            RuleFor(x => x.Categoria)
                .NotEmpty().WithMessage("A categoria do lançamento é obrigatória.")
                .Must(BeValidCategoriaLancamento).WithMessage($"A categoria do lançamento deve ser um dos valores válidos: {string.Join(", ", CategoriaLancamento.CategoriasValidas.Select(c => c.Valor))}.");

            // REQ-FLW-LAN-HU-002 (História de Usuário, observações opcionais com limite)
            RuleFor(x => x.Observacoes)
                .MaximumLength(500).WithMessage("As observações não podem exceder 500 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Observacoes));

            // NFR-OBS-001: O Correlation ID é obrigatório para rastreabilidade.
            RuleFor(x => x.CorrelationId)
                .NotEmpty().WithMessage("O Correlation ID é obrigatório para rastreabilidade.");
        }

        /// <summary>
        /// Verifica se o valor fornecido para o tipo de lançamento é válido.
        /// Utiliza o método <see cref="TipoLancamento.IsValid(string)"/>.
        /// </summary>
        /// <param name="tipo">O tipo de lançamento a ser validado.</param>
        /// <returns>`true` se o tipo for válido; caso contrário, `false`.</returns>
        private bool BeValidTipoLancamento(string tipo)
        {
            return TipoLancamento.IsValid(tipo);
        }

        /// <summary>
        /// Verifica se o valor fornecido para a categoria de lançamento é válido.
        /// Utiliza o método <see cref="CategoriaLancamento.IsValid(string)"/>.
        /// </summary>
        /// <param name="categoria">A categoria de lançamento a ser validada.</param>
        /// <returns>`true` se a categoria for válida; caso contrário, `false`.</returns>
        private bool BeValidCategoriaLancamento(string categoria)
        {
            return CategoriaLancamento.IsValid(categoria);
        }
    }
}