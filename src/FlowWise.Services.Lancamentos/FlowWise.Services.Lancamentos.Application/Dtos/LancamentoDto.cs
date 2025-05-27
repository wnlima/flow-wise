namespace FlowWise.Services.Lancamentos.Application.Dtos
{
    /// <summary>
    /// Representa um Data Transfer Object (DTO) para lançamentos financeiros
    /// na camada de Aplicação. Utilizado pelos Query Handlers para retornar
    /// dados que serão posteriormente mapeados para as respostas da API.
    /// </summary>
    /// <remarks>
    /// Este DTO garante que a camada de Aplicação seja independente da camada de Apresentação (API),
    /// aderindo aos princípios da Clean Architecture.
    /// </remarks>
    public class LancamentoDto
    {
        /// <summary>
        /// O identificador único do lançamento.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// O valor do lançamento.
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// A data em que o lançamento ocorreu.
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// A descrição do lançamento.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// O tipo do lançamento (ex: "Credito", "Debito").
        /// </summary>
        public string Tipo { get; set; } = string.Empty;

        /// <summary>
        /// A categoria do lançamento (ex: "Alimentacao", "Salario").
        /// </summary>
        public string Categoria { get; set; } = string.Empty;

        /// <summary>
        /// Observações adicionais sobre o lançamento.
        /// </summary>
        public string? Observacoes { get; set; }

        /// <summary>
        /// O identificador de correlação associado à criação ou última operação neste lançamento.
        /// </summary>
        public string? CorrelationId { get; set; }
    }
}