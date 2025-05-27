using FluentValidation;
using FlowWise.Services.Lancamentos.Application.Commands;
using FlowWise.Services.Lancamentos.Domain.ValueObjects;

namespace FlowWise.Services.Lancamentos.Application.Validators;

/// <summary>
/// Validador para o comando <see cref="UpdateLancamentoCommand"/>.
/// Garante que os dados fornecidos para a atualização de um lançamento são válidos
/// antes que a lógica de negócio seja executada.
/// </summary>
/// <remarks>
/// [Boas Práticas]: Aplicação rigorosa de SOLID e Clean Architecture através de validação de comandos.
/// [NFR-SEG-003]: Prevenção de ataques de injeção e dados inválidos (OWASP Top 10 - Validação de Entrada).
/// [TDD]: Testes unitários (UpdateLancamentoCommandValidatorTests) cobrem estas regras de validação.
/// </remarks>
public class UpdateLancamentoCommandValidator : AbstractValidator<UpdateLancamentoCommand>
{
    public UpdateLancamentoCommandValidator()
    {
        // [Requirement: HU-004] O ID do lançamento é obrigatório para identificação.
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID do lançamento é obrigatório.");

        // [Requirement: RN-006] O valor deve ser maior que zero.
        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("O valor do lançamento deve ser maior que zero. [RN-006]");

        // [Requirement: RN-002] A data do lançamento não pode ser futura.
        RuleFor(x => x.Data)
            .Must(date => date.Date <= DateTime.Today.Date).WithMessage("A data do lançamento não pode ser futura. [RN-002]");

        // [Requirement: HU-004] A descrição é obrigatória e tem um limite de caracteres.
        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição do lançamento é obrigatória.")
            .MaximumLength(255).WithMessage("A descrição do lançamento não pode exceder 255 caracteres.");

        // [Requirement: HU-004] O tipo deve ser "Credito" ou "Debito".
        // A regra RN-005 (tipo não pode ser alterado) será verificada na camada de domínio/handler,
        // aqui garantimos apenas que o valor fornecido seja um tipo válido.
        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("O tipo de lançamento é obrigatório.")
            .Must(BeValidTipoLancamento).WithMessage("O tipo de lançamento deve ser 'Credito' ou 'Debito'.");

        // [Requirement: HU-004] A categoria é obrigatória e deve ser um dos valores definidos.
        RuleFor(x => x.Categoria)
            .NotEmpty().WithMessage("A categoria do lançamento é obrigatória.")
            .Must(BeValidCategoriaLancamento).WithMessage($"A categoria do lançamento deve ser um dos valores válidos: {string.Join(", ", CategoriaLancamento.CategoriasValidas.Select(c => c.Valor))}.");

        // [Requirement: HU-004] Observações são opcionais, mas têm um limite de caracteres se fornecidas.
        RuleFor(x => x.Observacoes)
            .MaximumLength(500).WithMessage("As observações não podem exceder 500 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));

        // [Requirement: NFR-OBS-001] O Correlation ID é obrigatório para rastreabilidade.
        RuleFor(x => x.CorrelationId)
            .NotEmpty().WithMessage("O Correlation ID é obrigatório para rastreabilidade.");
    }

    /// <summary>
    /// Método de validação customizado para o tipo de lançamento.
    /// </summary>
    private bool BeValidTipoLancamento(string tipo)
    {
        return TipoLancamento.IsValid(tipo);
    }

    /// <summary>
    /// Método de validação customizado para a categoria de lançamento.
    /// </summary>
    private bool BeValidCategoriaLancamento(string categoria)
    {
        return CategoriaLancamento.IsValid(categoria);
    }
}
