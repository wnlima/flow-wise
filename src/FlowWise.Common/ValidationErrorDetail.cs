namespace FlowWise.Common;

/// <summary>
/// Detalhes de um erro de validação específico, indicando a propriedade e a mensagem de erro.
/// Utilizado dentro de <see cref="ApiResponse.Errors"/>.
/// </summary>
public class ValidationErrorDetail
{
    /// <summary>
    /// O nome da propriedade que causou o erro de validação.
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;
    /// <summary>
    /// A mensagem de erro de validação associada à propriedade.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}