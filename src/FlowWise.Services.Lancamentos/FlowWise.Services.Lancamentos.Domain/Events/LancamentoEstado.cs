namespace FlowWise.Services.Lancamentos.Domain.Events;

/// <summary>
/// Representa o estado de um lançamento em um determinado momento (antes ou depois da atualização).
/// </summary>
public record LancamentoEstado
{
    public decimal Valor { get; init; }
    public string Tipo { get; init; }
    public DateTime Data { get; init; }
    public string Descricao { get; init; }
    public string Categoria { get; init; }
    public string? Observacoes { get; init; }
}
