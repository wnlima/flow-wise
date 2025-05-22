using FlowWise.Services.Lancamentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Configuração de mapeamento para a entidade <see cref="Lancamento"/> no Entity Framework Core.
/// </summary>
/// <remarks>
/// Define como as propriedades da entidade <see cref="Lancamento"/> são mapeadas para as colunas da tabela no banco de dados.
/// REQ-FLW-INF-001: Mapeamento de entidades para o PostgreSQL.
/// ADR-PERSISTENCE-001: Decisão de usar Entity Framework Core para persistência.
/// </remarks>
public class LancamentoConfiguration : IEntityTypeConfiguration<Lancamento>
{
    /// <summary>
    /// Configura a entidade <see cref="Lancamento"/>.
    /// </summary>
    /// <param name="builder">O construtor de entidade para configurar.</param>
    public void Configure(EntityTypeBuilder<Lancamento> builder)
    {
        /// <remarks>
        /// REQ-FLW-DB-001: A chave primária deve ser um GUID.
        /// </remarks>
        builder.HasKey(l => l.Id);

        /// <remarks>
        /// REQ-FLW-DB-002: O valor do lançamento deve ter precisão monetária (decimal).
        /// RN-006: Validação de Valor.
        /// </remarks>
        builder.Property(l => l.Valor)
            .IsRequired()
            .HasColumnType("numeric(18,2)"); // Usando numeric para precisão monetária

        /// <remarks>
        /// REQ-FLW-DB-003: A descrição deve ter um tamanho máximo.
        /// </remarks>
        builder.Property(l => l.Descricao)
            .HasMaxLength(255)
            .IsRequired();

        /// <remarks>
        /// REQ-FLW-DB-004: Data do lançamento é obrigatória.
        /// RN-002: Validação de Data de Lançamento.
        /// </remarks>
        builder.Property(l => l.Data)
            .IsRequired();

        /// <remarks>
        /// REQ-FLW-DB-005: Observações são opcionais.
        /// </remarks>
        builder.Property(l => l.Observacoes)
            .HasMaxLength(500);

        /// <remarks>
        /// REQ-FLW-DB-006: Mapeamento do Value Object TipoLancamento.
        /// </remarks>
        builder.OwnsOne(l => l.Tipo, typeBuilder =>
        {
            typeBuilder.Property(t => t.Valor)
                .HasColumnName("Tipo")
                .IsRequired()
                .HasMaxLength(50);
        });

        /// <remarks>
        /// REQ-FLW-DB-007: Mapeamento do Value Object CategoriaLancamento.
        /// </remarks>
        builder.OwnsOne(l => l.Categoria, categoryBuilder =>
        {
            categoryBuilder.Property(c => c.Valor)
                .HasColumnName("Categoria")
                .IsRequired()
                .HasMaxLength(100);
        });

        /// <remarks>
        /// REQ-FLW-DB-008: Propriedades de auditoria. DataCriacao é obrigatória.
        /// NFR-SEG-005: Auditoria Completa de Operações.
        /// </remarks>
        builder.Property(l => l.DataCriacao)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        /// <remarks>
        /// REQ-FLW-DB-009: Propriedades de auditoria. DataAtualizacao é opcional.
        /// NFR-SEG-005: Auditoria Completa de Operações.
        /// </remarks>
        builder.Property(l => l.DataAtualizacao)
            .IsRequired(false)
            .HasColumnType("timestamp with time zone");

        /// <remarks>
        /// REQ-FLW-DB-010: Propriedade para exclusão lógica.
        /// </remarks>
        builder.Property(l => l.IsDeleted)
            .IsRequired();

        /// <remarks>
        /// REQ-FLW-DB-011: Ignora eventos de domínio, eles são tratados separadamente.
        /// ADR-DDD-003: Tratamento de Eventos de Domínio via MediatR/MassTransit.
        /// </remarks>
        builder.Ignore(l => l.DomainEvents);

        /// <remarks>
        /// REQ-FLW-DB-012: Nome da tabela no banco de dados.
        /// </remarks>
        builder.ToTable("Lancamentos");
    }
}