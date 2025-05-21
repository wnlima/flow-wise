using FlowWise.Services.Lancamentos.Domain.Exceptions;
using FlowWise.Services.Lancamentos.Domain.ValueObjects;
using FlowWise.Services.Lancamentos.Domain.Events;

namespace FlowWise.Services.Lancamentos.Domain.Entities;

/// <summary>
/// [DDD] Aggregate Root: Representa um lançamento de caixa (crédito ou débito).
/// Esta entidade é a raiz do agregado e garante a consistência das regras de negócio
/// relacionadas a um lançamento.
/// </summary>
/// <remarks>
/// [HU-001]: Um lançamento pode ser de débito ou crédito.
/// [HU-002]: Lançamentos podem ser registrados e ter seus valores e tipos alterados.
/// [HU-003]: Lançamentos podem ser excluídos.
/// </remarks>
public class Lancamento
{
    /// <summary>
    /// O identificador único do lançamento.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// O valor monetário do lançamento. Deve ser maior que zero.
    /// </summary>
    public decimal Valor { get; set; }
    /// <summary>
    /// A data em que o lançamento ocorreu. Não pode ser uma data futura.
    /// </summary>
    public DateTime Data { get; set; }
    /// <summary>
    /// A descrição do lançamento.
    /// </summary>
    public string Descricao { get; set; }
    /// <summary>
    /// O tipo do lançamento (Crédito ou Débito), representado por um Value Object.
    /// </summary>
    public TipoLancamento Tipo { get; set; }
    /// <summary>
    /// A categoria do lançamento (ex: Salário, Aluguel, Alimentação), representado por um Value Object.
    /// </summary>
    public CategoriaLancamento Categoria { get; set; }
    /// <summary>
    /// Observações adicionais do lançamento. Pode ser nulo.
    /// </summary>
    public string? Observacoes { get; set; }
    /// A data e hora (UTC) de criação do lançamento.
    /// [NFR-SEG-005]: Para fins de auditoria.
    /// </summary>
    public DateTime DataCriacao { get; set; }
    /// <summary>
    /// A data e hora (UTC) da última atualização do lançamento.
    /// [NFR-SEG-005]: Para fins de auditoria.
    /// </summary>
    public DateTime? DataAtualizacao { get; set; }

    // Nova propriedade para exclusão lógica (se necessário)
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Coleção de eventos de domínio a serem disparados após a persistência da entidade.
    /// [DDD]: Permite o disparo de eventos de domínio que notificam outras partes do sistema.
    /// </summary>
    public readonly List<IDomainEvent> DomainEvents = new();

    /// <summary>
    /// Construtor privado para uso do Entity Framework Core e para forçar o uso do Factory Method <see cref="Create"/>.
    /// </summary>
    private Lancamento() { }

    /// <summary>
    /// [DDD] Factory Method: Cria uma nova instância de <see cref="Lancamento"/>.
    /// Garante a validade inicial dos dados do lançamento e registra o evento de domínio.
    /// </summary>
    /// <param name="valor">O valor do lançamento.</param>
    /// <param name="data">A data do lançamento.</param>
    /// <param name="descricao">A descrição do lançamento.</param>
    /// <param name="tipo">O tipo do lançamento (Crédito ou Débito).</param>
    /// <param name="categoria">A categoria do lançamento.</param>
    /// <param name="observacoes">Observações adicionais.</param>
    /// <param name="correlationId">O Correlation ID da operação.</param>
    /// <returns>Uma nova instância de <see cref="Lancamento"/>.</returns>
    /// <exception cref="LancamentoDomainException">Lançada se as regras de negócio forem violadas.</exception>
    public static Lancamento Create(decimal valor, DateTime data, string descricao, string tipo, string categoria, string? observacoes, string correlationId)
    {
        var newLancamento = new Lancamento
        {
            Id = Guid.NewGuid(),
            DataCriacao = DateTime.UtcNow,
            IsDeleted = false
        };

        // Aplica as propriedades usando métodos que encapsulam validações
        newLancamento.SetValor(valor);
        newLancamento.SetData(data);
        newLancamento.SetDescricao(descricao);
        newLancamento.SetTipo(tipo);
        newLancamento.SetCategoria(categoria);
        newLancamento.SetObservacoes(observacoes);
        newLancamento.AddDomainEvent(new LancamentoRegistradoEvent(newLancamento.Id, newLancamento.Valor, newLancamento.Tipo.Valor, newLancamento.Data, correlationId));

        return newLancamento;
    }

    /// <summary>
    /// [DDD] Comportamento: Atualiza os dados de um lançamento existente.
    /// Dispara o evento de domínio <see cref="LancamentoAtualizadoEvent"/> se houver mudanças.
    /// </summary>
    /// <param name="novoValor">O novo valor do lançamento.</param>
    /// <param name="novaData">A nova data do lançamento.</param>
    /// <param name="novaDescricao">A nova descrição do lançamento.</param>
    /// <param name="novoTipo">O novo tipo do lançamento.</param>
    /// <param name="novaCategoria">A nova categoria do lançamento.</param>
    /// <param name="novasObservacoes">As novas observações do lançamento.</param>
    /// <param name="correlationId">O Correlation ID da operação.</param>
    /// <exception cref="LancamentoDomainException">Lançada se as regras de negócio forem violadas.</exception>
    public void Update(decimal novoValor, DateTime novaData, string novaDescricao, string novoTipo, string novaCategoria, string? novasObservacoes, string correlationId)
    {
        // RN-005: Imutabilidade do Tipo de Lançamento
        if (!Tipo.Valor.Equals(novoTipo, StringComparison.OrdinalIgnoreCase))
        {
            throw new LancamentoDomainException("O tipo de um lançamento não pode ser alterado após o registro. [RN-005]");
        }

        // Captura o estado atual do lançamento ANTES das modificações
        var lancamentoAntigo = new LancamentoSnapshot(
            Id,
            Valor,
            Tipo.Valor,
            Data,
            Descricao,
            Categoria.Valor,
            Observacoes
        );

        bool changed = false;

        if (Valor != novoValor)
        {
            SetValor(novoValor);
            changed = true;
        }
        if (Data.Date != novaData.Date)
        {
            SetData(novaData);
            changed = true;
        }
        if (Descricao != novaDescricao)
        {
            SetDescricao(novaDescricao);
            changed = true;
        }
        // O tipo não pode ser alterado (RN-005)
        if (Categoria.Valor != novaCategoria)
        {
            SetCategoria(novaCategoria);
            changed = true;
        }
        if (Observacoes != novasObservacoes)
        {
            SetObservacoes(novasObservacoes);
            changed = true;
        }

        if (changed)
        {
            DataAtualizacao = DateTime.UtcNow;
            var lancamentoNovo = new LancamentoSnapshot(
                Id,
                Valor,
                Tipo.Valor,
                Data,
                Descricao,
                Categoria.Valor,
                Observacoes
            );

            AddDomainEvent(new LancamentoAtualizadoEvent(Id, lancamentoAntigo, lancamentoNovo, correlationId));
        }
    }

    /// <summary>
    /// [DDD] Comportamento: Marca o lançamento como excluído (exclusão lógica) e dispara o evento de domínio <see cref="LancamentoExcluidoEvent"/>.
    /// </summary>
    /// <param name="correlationId">O Correlation ID da operação.</param>
    public void Delete(string correlationId)
    {
        if (IsDeleted)
            throw new LancamentoDomainException("Lançamento já está marcado como excluído.");

        if (Data.Date < DateTime.Today.Date)
            throw new LancamentoDomainException("Lançamento só pode ser excluído se a data for igual ao dia atual.");

        IsDeleted = true; // Marca como excluído logicamente
        DataAtualizacao = DateTime.UtcNow; // Atualiza data de modificação para auditoria

        AddDomainEvent(new LancamentoExcluidoEvent(Id, Valor, Tipo.Valor, Data, correlationId));
    }

    /// <summary>
    /// Adiciona um evento de domínio à lista para ser disparado.
    /// </summary>
    /// <param name="eventItem">O evento de domínio a ser adicionado.</param>
    internal void AddDomainEvent(IDomainEvent eventItem)
    {
        DomainEvents.Add(eventItem);
    }

    /// <summary>
    /// Limpa todos os eventos de domínio registrados.
    /// Chamado após a persistência bem-sucedida dos eventos.
    /// </summary>
    public void ClearDomainEvents()
    {
        DomainEvents.Clear();
    }

    private void SetValor(decimal valor)
    {
        if (valor <= 0)
            throw new LancamentoDomainException("O valor do lançamento deve ser maior que zero. [RN-006]");
        Valor = valor;
    }

    private void SetData(DateTime data)
    {
        if (data.Date > DateTime.Today.Date)
            throw new LancamentoDomainException("A data do lançamento não pode ser futura. [RN-002]");
        Data = data.Date;
    }

    private void SetDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new LancamentoDomainException("A descrição do lançamento é obrigatória.");
        if (descricao.Length > 255)
            throw new LancamentoDomainException("A descrição do lançamento não pode exceder 255 caracteres.");
        Descricao = descricao.Trim();
    }

    private void SetTipo(string tipo)
    {
        Tipo = TipoLancamento.From(tipo); // Delega a validação para o Value Object
    }

    private void SetCategoria(string categoria)
    {
        Categoria = CategoriaLancamento.From(categoria); // Delega a validação para o Value Object
    }

    private void SetObservacoes(string? observacoes)
    {
        if (observacoes != null && observacoes.Trim().Length > 500)
            throw new LancamentoDomainException("As observações não podem exceder 500 caracteres.");
        Observacoes = observacoes?.Trim();
    }
}
