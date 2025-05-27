using System.ComponentModel.DataAnnotations;

namespace FlowWise.Services.Lancamentos.Api.Requests
{
    public class CreateLancamentoRequest
    {
        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser positivo.")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "O tipo de lançamento é obrigatório.")]
        [RegularExpression("^(Debito|Credito)$", ErrorMessage = "O tipo deve ser 'Debito' ou 'Credito'.")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data é obrigatória.")]
        // Validação da data no passado/presente será na lógica de negócio/handler
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "A descrição deve ter entre 5 e 255 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public string Categoria { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres.")]
        public string? Observacoes { get; set; }
    }
}