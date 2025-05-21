using FlowWise.Common.ValueObjects;

namespace FlowWise.Services.Lancamentos.Domain.ValueObjects;

/// <summary>
/// [DDD] Value Object: Representa uma "fotografia" imutável de um lançamento em um dado momento.
/// Utilizado em eventos de domínio para registrar o estado antigo e novo de um lançamento,
/// facilitando a auditoria e a consistência eventual em sistemas distribuídos.
/// </summary>
/// <remarks>
/// [NFR-SEG-005]: Contribui para a rastreabilidade e auditabilidade das operações de atualização.
/// </remarks>
public class LancamentoSnapshot : ValueObject
{
    /// <summary>
    /// O identificador único do lançamento.
    /// </summary>
    public Guid Id { get; init; }
    /// <summary>
    /// O valor monetário do lançamento nesta fotografia.
    /// </summary>
    public decimal Valor { get; init; }
    /// <summary>
    /// O tipo do lançamento (Crédito ou Débito) nesta fotografia.
    /// </summary>
    public string Tipo { get; init; }
    /// <summary>
    /// A data do lançamento nesta fotografia.
    /// </summary>
    public DateTime Data { get; init; }
    /// <summary>
    /// A descrição do lançamento nesta fotografia.
    /// </summary>
    public string Descricao { get; init; }
    /// <summary>
    /// A categoria do lançamento nesta fotografia.
    /// </summary>
    public string Categoria { get; init; }
    /// <summary>
    /// Observações adicionais do lançamento nesta fotografia.
    /// </summary>
    public string? Observacoes { get; init; }

    /// <summary>
    /// Construtor público para criar uma nova instância de <see cref="LancamentoSnapshot"/>.
    /// </summary>
    /// <param name="id">O ID do lançamento.</param>
    /// <param name="valor">O valor do lançamento.</param>
    /// <param name="tipo">O tipo do lançamento.</param>
    /// <param name="data">A data do lançamento.</param>
    /// <param name="descricao">A descrição do lançamento.</param>
    /// <param name="categoria">A categoria do lançamento.</param>
    /// <param name="observacoes">Observações do lançamento.</param>
    public LancamentoSnapshot(Guid id, decimal valor, string tipo, DateTime data, string descricao, string categoria, string? observacoes)
    {
        Id = id;
        Valor = valor;
        Tipo = tipo;
        Data = data.Date;
        Descricao = descricao;
        Categoria = categoria;
        Observacoes = observacoes;
    }

    /// <summary>
    /// Retorna os componentes que definem a igualdade deste Value Object.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
        yield return Valor;
        yield return Tipo;
        yield return Data.Date;
        yield return Descricao;
        yield return Categoria;
        yield return Observacoes ?? string.Empty;
    }
}
