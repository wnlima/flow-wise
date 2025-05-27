using FlowWise.Services.Consolidacao.Domain.Exceptions;

namespace FlowWise.Services.Consolidacao.Domain.Entities
{
    /// <summary>
    /// [DDD] Entidade: Representa o saldo consolidado de caixa para um dia específico.
    /// É uma projeção otimizada para leitura, gerada a partir dos lançamentos.
    /// </summary>
    /// <remarks>
    /// [HU-006]: Utilizado para visualização do saldo diário consolidado.
    /// [RN-003]: O saldo diário consolidado para uma data X é calculado com base na soma de todos os lançamentos finalizados até o final do dia X-1 (dia anterior).
    /// </remarks>
    public class SaldoDiario
    {
        /// <summary>
        /// A data do dia consolidado a que o saldo se refere (apenas a parte da data).
        /// É a chave primária desta entidade.
        /// </summary>
        public DateTime Data { get; set; }
        /// <summary>
        /// O saldo total líquido para o dia, calculado como TotalCréditos - TotalDébitos.
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
        /// [NFR-SEG-005]: Propriedade para fins de auditoria.
        /// </summary>
        public DateTime UltimaAtualizacao { get; set; }

        /// <summary>
        /// Construtor privado para uso do Entity Framework Core e para forçar o uso do Factory Method <see cref="Create"/>.
        /// </summary>
        private SaldoDiario() { }

        /// <summary>
        /// [DDD] Factory Method: Cria uma nova instância de <see cref="SaldoDiario"/> para uma data específica.
        /// </summary>
        /// <param name="data">A data para a qual o saldo diário será criado. Não pode ser futura.</param>
        /// <returns>Uma nova instância de <see cref="SaldoDiario"/>.</returns>
        /// <exception cref="ConsolidacaoDomainException">Lançada se a data for futura, violando a regra RN-003.</exception>
        public static SaldoDiario Create(DateTime data)
        {
            // RN-003: O consolidado é sempre de D-1. A projeção será para a data passada.
            if (data.Date > DateTime.Today.Date)
                throw new ConsolidacaoDomainException("A data para consolidação não pode ser futura.");

            return new SaldoDiario
            {
                Data = data.Date, // Garante que apenas a data é armazenada
                SaldoTotal = 0,
                TotalCreditos = 0,
                TotalDebitos = 0,
                UltimaAtualizacao = DateTime.UtcNow // Registra o momento da criação
            };
        }

        /// <summary>
        /// Aplica o impacto de um lançamento (crédito ou débito) no saldo diário.
        /// Atualiza os totais de créditos/débitos e o saldo total.
        /// </summary>
        /// <param name="valor">O valor do lançamento a ser aplicado.</param>
        /// <param name="tipo">O tipo do lançamento ("Credito" ou "Debito").</param>
        /// <exception cref="ConsolidacaoDomainException">Lançada se o tipo de lançamento for inválido.</exception>
        public void AplicarLancamento(string tipo, decimal valor)
        {
            if (tipo.Equals("Credito", StringComparison.OrdinalIgnoreCase))
            {
                TotalCreditos += valor;
            }
            else if (tipo.Equals("Debito", StringComparison.OrdinalIgnoreCase))
            {
                TotalDebitos += valor;
            }
            else
            {
                // Isso nunca deveria acontecer se o TipoLancamento do Lançamento for validado na origem.
                throw new ConsolidacaoDomainException("Tipo de lançamento inválido para aplicação de saldo.");
            }

            SaldoTotal = TotalCreditos - TotalDebitos;
            UltimaAtualizacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Reverte o impacto de um lançamento (crédito ou débito) do saldo diário.
        /// Usado em cenários de exclusão ou atualização de lançamentos.
        /// </summary>
        /// <param name="valor">O valor do lançamento a ser revertido.</param>
        /// <param name="tipo">O tipo do lançamento ("Credito" ou "Debito").</param>
        /// <exception cref="ConsolidacaoDomainException">Lançada se o tipo de lançamento for inválido.</exception>
        public void ReverterLancamento(string tipo, decimal valor)
        {
            if (tipo.Equals("Credito", StringComparison.OrdinalIgnoreCase))
            {
                TotalCreditos -= valor;
            }
            else if (tipo.Equals("Debito", StringComparison.OrdinalIgnoreCase))
            {
                TotalDebitos -= valor;
            }
            else
            {
                throw new ConsolidacaoDomainException("Tipo de lançamento inválido para reversão de saldo.");
            }

            SaldoTotal = TotalCreditos - TotalDebitos;
            UltimaAtualizacao = DateTime.UtcNow;
        }
    }
}