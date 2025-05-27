namespace FlowWise.Services.Consolidacao.Application.Dtos;

/// <summary>
/// DTO (Data Transfer Object) para a projeção de saldo diário consolidado na camada de Aplicação.
/// Utilizado pelos Query Handlers para retornar dados que serão posteriormente mapeados para as respostas da API.
/// </summary>
/// <remarks>
/// Este DTO garante que a camada de Aplicação seja independente da camada de Apresentação (API),
/// aderindo aos princípios da Clean Architecture.
/// [HU-006]: Usado para apresentar os dados de saldo diário.
/// </remarks>
public class SaldoDiarioDto
{
    /// <summary>
    /// A data do dia consolidado a que o saldo se refere (apenas a parte da data).
    /// </summary>
    public DateTime Data { get; set; }
    /// <summary>
    /// O saldo total líquido para o dia, calculado como TotalCreditos - TotalDebitos.
    /// </summary>
    public decimal SaldoTotal { get; set; }
    /// <summary>
    /// O valor total de créditos (entradas) para o dia.
    /// </summary>
    public decimal TotalCreditos { get; set; }
    /// <summary>
    /// O valor total de débitos (saídas) para o dia.
    /// </summary>
    public decimal TotalDebitos { get; set; }
    /// <summary>
    /// A data e hora (UTC) da última atualização deste registro de saldo.
    /// </summary>
    public DateTime UltimaAtualizacao { get; set; }
}