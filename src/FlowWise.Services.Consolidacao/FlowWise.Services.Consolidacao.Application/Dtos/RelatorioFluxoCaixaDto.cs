namespace FlowWise.Services.Consolidacao.Application.Dtos;

/// <summary>
/// DTO (Data Transfer Object) para o relatório de fluxo de caixa por período na camada de Aplicação.
/// Utilizado pelos Query Handlers para retornar dados que serão posteriormente mapeados para as respostas da API.
/// </summary>
/// <remarks>
/// Este DTO garante que a camada de Aplicação seja independente da camada de Apresentação (API),
/// aderindo aos princípios da Clean Architecture.
/// [HU-007]: Usado para apresentar os dados do relatório de fluxo de caixa.
/// </remarks>
public class RelatorioFluxoCaixaDto
{
    /// <summary>
    /// A data de início do período do relatório (apenas a parte da data).
    /// </summary>
    public DateTime DataInicio { get; set; }
    /// <summary>
    /// A data de fim do período do relatório (apenas a parte da data).
    /// </summary>
    public DateTime DataFim { get; set; }
    /// <summary>
    /// O saldo de caixa no início do período do relatório.
    /// </summary>
    public decimal SaldoInicial { get; set; }
    /// <summary>
    /// O valor total de créditos (entradas) acumulado no período do relatório.
    /// </summary>
    public decimal TotalCreditos { get; set; }
    /// <summary>
    /// O valor total de débitos (saídas) acumulado no período do relatório.
    /// </summary>
    public decimal TotalDebitos { get; set; }
    /// <summary>
    /// O saldo de caixa no final do período do relatório.
    /// </summary>
    public decimal SaldoFinal { get; set; }
}