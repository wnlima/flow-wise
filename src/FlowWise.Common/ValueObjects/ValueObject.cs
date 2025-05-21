namespace FlowWise.Common.ValueObjects;

/// <summary>
/// Fornece uma implementação padrão de igualdade baseada nos componentes.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Retorna os componentes que definem a igualdade deste Value Object.
    /// </summary>
    /// <returns>Uma coleção de objetos que compõem a identidade do Value Object.</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>
    /// Compara este Value Object com outro objeto para igualdade.
    /// </summary>
    /// <param name="obj">O objeto a ser comparado.</param>
    /// <returns>True se os objetos forem iguais; caso contrário, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Retorna o código hash para este Value Object.
    /// </summary>
    /// <returns>O código hash.</returns>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// Sobrecarga do operador de igualdade (==) para Value Objects.
    /// </summary>
    public static bool operator ==(ValueObject left, ValueObject right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
        {
            return true;
        }

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
        {
            return false;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// Sobrecarga do operador de desigualdade (!=) para Value Objects.
    /// </summary>
    public static bool operator !=(ValueObject left, ValueObject right)
    {
        return !(left == right);
    }
}