using FluentValidation;
using FlowWise.Services.Lancamentos.Application.Commands;

namespace FlowWise.Services.Lancamentos.Application.Validators
{
    /// <summary>
    /// [FluentValidation] Validador para o comando <see cref="DeleteLancamentoCommand"/>.
    /// Garante que o ID do lançamento a ser excluído é válido.
    /// </summary>
    public class DeleteLancamentoCommandValidator : AbstractValidator<DeleteLancamentoCommand>
    {
        public DeleteLancamentoCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O ID do lançamento é obrigatório para exclusão.");
        }
    }
}