using FlowWise.Common.Models; // Para BaseCommand

namespace FlowWise.Services.Lancamentos.Application.Commands
{
    /// <summary>
    /// [CQRS] Comando: Representa a requisição para excluir um lançamento de caixa existente.
    /// Herda de <see cref="BaseCommand{TResponse}"/> para inclusão do CorrelationId.
    /// </summary>
    /// <remarks>
    /// [HU-003]: Permite a exclusão de lançamentos.
    /// </remarks>
    public class DeleteLancamentoCommand : BaseCommand<bool> // Retorna true se excluído, false se não encontrado
    {
        /// <summary>
        /// O identificador único do lançamento a ser excluído.
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid Id { get; set; }
        public DeleteLancamentoCommand(string correlationId) : base(correlationId) { }
    }
}