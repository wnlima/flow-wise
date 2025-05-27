using FlowWise.Common.Models; // Para BaseCommand

namespace FlowWise.Services.Lancamentos.Application.Commands
{
    /// <summary>
    /// [CQRS] Comando: Representa a requisição para criar um novo lançamento de caixa.
    /// Herda de <see cref="BaseCommand{TResponse}"/> para inclusão do CorrelationId.
    /// </summary>
    /// <remarks>
    /// [HU-001]: Permite o registro de lançamentos de débito e crédito.
    /// </remarks>
    public class CreateLancamentoCommand : BaseCommand<Guid> // Retorna o ID do lançamento criado
    {
        /// <summary>
        /// O valor monetário do lançamento.
        /// </summary>
        /// <example>100.50</example>
        public decimal Valor { get; set; }
        /// <summary>
        /// A data em que o lançamento ocorreu.
        /// </summary>
        /// <example>2024-05-23</example>
        public DateTime Data { get; set; }
        /// <summary>
        /// A descrição do lançamento.
        /// </summary>
        /// <example>Pagamento de Aluguel</example>
        public string Descricao { get; set; } = string.Empty;
        /// <summary>
        /// O tipo do lançamento (Crédito ou Débito).
        /// </summary>
        /// <example>Debito</example>
        public string Tipo { get; set; } = string.Empty;
        /// <summary>
        /// A categoria do lançamento (ex: Salário, Aluguel).
        /// </summary>
        /// <example>Aluguel</example>
        public string Categoria { get; set; } = string.Empty;
        /// <summary>
        /// Observações adicionais do lançamento. Pode ser nulo.
        /// </summary>
        public string? Observacoes { get; set; } = string.Empty;
        public CreateLancamentoCommand(string correlationId) : base(correlationId) { }
    }
}