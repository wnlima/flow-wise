using FlowWise.Services.Consolidacao.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowWise.Services.Consolidacao.Infrastructure.Persistence;

public class SaldoDiarioConfiguration : IEntityTypeConfiguration<SaldoDiario>
{
       public void Configure(EntityTypeBuilder<SaldoDiario> builder)
       {
              builder.ToTable("SaldosDiarios");

              builder.HasKey(sd => sd.Data);

              builder.Property(s => s.Data)
                     .HasColumnType("timestamp with time zone")
                     .IsRequired();

              builder.Property(sd => sd.SaldoTotal)
                     .IsRequired()
                     .HasColumnType("numeric(18,2)");

              builder.Property(sd => sd.TotalCreditos)
                     .IsRequired()
                     .HasColumnType("numeric(18,2)");

              builder.Property(sd => sd.TotalDebitos)
                     .IsRequired()
                     .HasColumnType("numeric(18,2)");

              builder.Property(sd => sd.UltimaAtualizacao)
                     .IsRequired()
                     .HasColumnType("timestamp with time zone")
                     .IsConcurrencyToken();
       }
}