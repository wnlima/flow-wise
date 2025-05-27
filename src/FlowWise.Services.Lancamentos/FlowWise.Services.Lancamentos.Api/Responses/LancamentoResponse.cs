namespace FlowWise.Services.Lancamentos.Api.Responses
{
    /// <summary>
    /// Representa o objeto de resposta para um lançamento financeiro,
    /// utilizado para serializar os dados de um lançamento para o cliente da API.
    /// </summary>
    /// <remarks>
    /// REQ-FLW-LAN-004: O sistema deve permitir a consulta de um lançamento específico.
    /// HU-001: Como usuário do sistema, quero registrar, listar e consultar lançamentos financeiros, para ter controle do meu fluxo de caixa diário.
    /// NFR-OBS-001: O sistema deve registrar um Correlation ID para rastreabilidade de requisições.
    /// </remarks>
    public class LancamentoResponse
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
    }
}