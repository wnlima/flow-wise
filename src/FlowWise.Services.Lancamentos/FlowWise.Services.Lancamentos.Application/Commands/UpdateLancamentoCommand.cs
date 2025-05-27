using FlowWise.Common.Models; // Para BaseCommand

namespace FlowWise.Services.Lancamentos.Application.Commands;

/// <summary>
/// [CQRS] Comando: Representa a requisição para atualizar um lançamento de caixa existente.
/// Herda de <see cref="BaseCommand{TResponse}"/> para inclusão do CorrelationId.
/// </summary>
/// <remarks>
/// [HU-002]: Permite a alteração de valor e tipo de lançamentos.
/// </remarks>
public class UpdateLancamentoCommand : BaseCommand<bool> // Retorna true se atualizado, false se não
{
    /// <summary>
    /// O identificador único do lançamento a ser atualizado.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// O novo valor monetário do lançamento.
    /// </summary>
    public decimal Valor { get; set; }
    /// <summary>
    /// A nova data em que o lançamento ocorreu.
    /// </summary>
    public DateTime Data { get; set; }
    /// <summary>
    /// A nova descrição do lançamento.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;
    /// <summary>
    /// O novo tipo do lançamento (Crédito ou Débito).
    /// </summary>
    public string Tipo { get; set; } = string.Empty;
    /// <summary>
    /// A nova categoria do lançamento.
    /// </summary>
    public string Categoria { get; set; } = string.Empty;
    /// <summary>
    /// Observações adicionais do lançamento. Pode ser nulo.
    /// </summary>
    public string? Observacoes { get; set; } = string.Empty;
    public UpdateLancamentoCommand(string correlationId) : base(correlationId) { }
}