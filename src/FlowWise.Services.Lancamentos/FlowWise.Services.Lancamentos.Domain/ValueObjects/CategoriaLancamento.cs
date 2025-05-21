using FlowWise.Common.ValueObjects;
using FlowWise.Services.Lancamentos.Domain.Exceptions;

namespace FlowWise.Services.Lancamentos.Domain.ValueObjects
{
    /// <summary>
    /// [DDD] Value Object: Representa a categoria de um lançamento financeiro.
    /// Garante que apenas categorias válidas sejam criadas e que a comparação seja feita por valor.
    /// </summary>
    /// <remarks>
    /// [Boas Práticas]: Implementa o padrão Value Object do DDD, garantindo imutabilidade e validação de regras de negócio internas.
    /// [HU-002]: Lançamentos podem ter categorias para organização.
    /// [NFR-SEG-003]: Ajuda a prevenir dados inválidos, contribuindo para a segurança da entrada.
    /// </remarks>
    public class CategoriaLancamento : ValueObject
    {
        /// <summary>
        /// O valor string da categoria do lançamento (e.g., "Salario", "Aluguel").
        /// </summary>
        public string Valor { get; private set; }

        // --- Instâncias estáticas para categorias de débito comuns ---
        public static CategoriaLancamento Alimentacao => new CategoriaLancamento("Alimentacao");
        public static CategoriaLancamento Transporte => new CategoriaLancamento("Transporte");
        public static CategoriaLancamento Moradia => new CategoriaLancamento("Moradia");
        public static CategoriaLancamento Saude => new CategoriaLancamento("Saude");
        public static CategoriaLancamento Educacao => new CategoriaLancamento("Educacao");
        public static CategoriaLancamento Lazer => new CategoriaLancamento("Lazer");
        public static CategoriaLancamento ContasResidenciais => new CategoriaLancamento("Contas Residenciais");
        public static CategoriaLancamento Compras => new CategoriaLancamento("Compras");
        public static CategoriaLancamento OutrosDebitos => new CategoriaLancamento("Outros Debitos");

        // --- Instâncias estáticas para categorias de crédito comuns ---
        public static CategoriaLancamento Salario => new CategoriaLancamento("Salario");
        public static CategoriaLancamento Investimentos => new CategoriaLancamento("Investimentos");
        public static CategoriaLancamento ReceitasExtras => new CategoriaLancamento("Receitas Extras");
        public static CategoriaLancamento Vendas => new CategoriaLancamento("Vendas");
        public static CategoriaLancamento OutrosCreditos => new CategoriaLancamento("Outros Creditos");

        public static readonly List<CategoriaLancamento> CategoriasValidas = new List<CategoriaLancamento>
        {
            Alimentacao, Transporte, Moradia, Saude, Educacao, Lazer, ContasResidenciais, Compras, OutrosDebitos,
            Salario, Investimentos, ReceitasExtras, Vendas, OutrosCreditos
        };

        /// <summary>
        /// Construtor privado para garantir que as instâncias sejam criadas apenas através
        /// dos métodos estáticos ou do método 'From'.
        /// </summary>
        /// <param name="valor">O valor da categoria de lançamento.</param>
        private CategoriaLancamento(string valor)
        {
            // Validação interna: Garantir que o valor não seja nulo ou vazio no construtor.
            // A validação completa é feita no método From.
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new LancamentoDomainException("O valor da categoria de lançamento não pode ser nulo ou vazio.");
            }
            Valor = valor;
        }

        /// <summary>
        /// [DDD] Factory Method: Cria uma instância de <see cref="CategoriaLancamento"/> a partir de uma string.
        /// Valida se a string fornecida corresponde a uma categoria de lançamento válida.
        /// </summary>
        /// <param name="valor">A string que representa a categoria de lançamento.</param>
        /// <returns>Uma nova instância de <see cref="CategoriaLancamento"/>.</returns>
        /// <exception cref="LancamentoDomainException">Lançada se a categoria de lançamento for inválida.</exception>
        public static CategoriaLancamento From(string valor)
        {
            var categoria = CategoriasValidas.FirstOrDefault(c => c.Valor.Equals(valor ?? "", StringComparison.OrdinalIgnoreCase));

            if (categoria is null)
                throw new LancamentoDomainException("Categoria de lançamento inválida.");

            return categoria;
        }

        /// <summary>
        /// Verifica se a string fornecida representa uma categoria de lançamento válida.
        /// Usado principalmente para validações externas (e.g., FluentValidation).
        /// </summary>
        /// <param name="valor">A string a ser validada.</param>
        /// <returns>True se for uma categoria válida; caso contrário, False.</returns>
        public static bool IsValid(string valor)
        {
            return CategoriasValidas.Any(c => c.Valor.Equals(valor, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Retorna os componentes que definem a igualdade deste Value Object.
        /// Utilizado pela classe base <see cref="ValueObject"/> para comparação por valor.
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Valor.ToUpperInvariant();
        }

        /// <summary>
        /// Retorna a representação em string da categoria de lançamento.
        /// </summary>
        /// <returns>O valor string da categoria de lançamento.</returns>
        public override string ToString()
        {
            return Valor;
        }
    }
}