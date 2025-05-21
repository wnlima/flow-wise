using FlowWise.Common.ValueObjects; // 
using FlowWise.Services.Lancamentos.Domain.Exceptions;

namespace FlowWise.Services.Lancamentos.Domain.ValueObjects;

/// <summary>
/// [DDD] Value Object: Representa o tipo de um lançamento (Crédito ou Débito).
/// Garante que apenas tipos válidos sejam criados e que a comparação seja feita por valor.
/// </summary>
/// <remarks>
/// [Boas Práticas]: Implementa o padrão Value Object do DDD, garantindo imutabilidade e validação de regras de negócio internas.
/// [HU-001]: Um lançamento pode ser de débito ou crédito.
/// [NFR-SEG-003]: Ajuda a prevenir dados inválidos, contribuindo para a segurança da entrada.
/// </remarks>
public class TipoLancamento : ValueObject
{
    /// <summary>
    /// O valor string do tipo de lançamento (e.g., "Credito", "Debito").
    /// </summary>
    public string Valor { get; private set; }

    // Instâncias estáticas para os tipos de lançamento permitidos
    public static TipoLancamento Credito => new TipoLancamento("Credito");
    public static TipoLancamento Debito => new TipoLancamento("Debito");

    // Lista interna de todos os tipos válidos
    private static readonly List<TipoLancamento> _tiposValidos = new List<TipoLancamento> { Credito, Debito };

    /// <summary>
    /// Construtor privado para garantir que as instâncias sejam criadas apenas através
    /// dos métodos estáticos ou do método 'From'.
    /// </summary>
    /// <param name="valor">O valor do tipo de lançamento.</param>
    private TipoLancamento(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new LancamentoDomainException("O tipo de lançamento é obrigatório.");

        Valor = valor;
    }

    /// <summary>
    /// [DDD] Factory Method: Cria uma instância de <see cref="TipoLancamento"/> a partir de uma string.
    /// Valida se a string fornecida corresponde a um tipo de lançamento válido.
    /// </summary>
    /// <param name="valor">A string que representa o tipo de lançamento.</param>
    /// <returns>Uma nova instância de <see cref="TipoLancamento"/>.</returns>
    /// <exception cref="LancamentoDomainException">Lançada se o tipo de lançamento for inválido.</exception>
    public static TipoLancamento From(string valor)
    {
        var tipo = _tiposValidos.FirstOrDefault(t => t.Valor.Equals(valor ?? "", StringComparison.OrdinalIgnoreCase));

        if (tipo is null)
        {
            throw new LancamentoDomainException("O tipo de lançamento deve ser 'Credito' ou 'Debito'.");
        }
        return tipo;
    }

    /// <summary>
    /// Verifica se a string fornecida representa um tipo de lançamento válido.
    /// Usado principalmente para validações externas (e.g., FluentValidation).
    /// </summary>
    /// <param name="valor">A string a ser validada.</param>
    /// <returns>True se for um tipo válido; caso contrário, False.</returns>
    public static bool IsValid(string valor)
    {
        return _tiposValidos.Any(t => t.Valor.Equals(valor, StringComparison.OrdinalIgnoreCase));
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
    /// Retorna a representação em string do tipo de lançamento.
    /// </summary>
    /// <returns>O valor string do tipo de lançamento.</returns>
    public override string ToString()
    {
        return Valor;
    }
}