using FlowWise.Core;
using Serilog;
using FlowWise.Services.Lancamentos.Application.Commands;
using FlowWise.Services.Lancamentos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FlowWise.Services.Lancamentos.Domain.Interfaces;
using FlowWise.Services.Lancamentos.Api.Middleware;
using FlowWise.Services.Lancamentos.Domain.Events;
using FlowWise.Services.Lancamentos.Application.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithCorrelationIdHeader()
        .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adiciona os serviços comuns do Flow Wise Core (infraestrutura compartilhada ou padronizada)
builder.Services.AddFlowWiseCoreServices(builder.Configuration, typeof(CreateLancamentoCommand).Assembly);
builder.Services.ConfigureMassTransitWithRabbitMq(builder.Configuration, cfg =>
{
    cfg.Message<LancamentoRegistradoEvent>(m => m.SetEntityName("ex.flow-wise.lancamentos.events"));
    cfg.Message<LancamentoAtualizadoEvent>(m => m.SetEntityName("ex.flow-wise.lancamentos.events"));
    cfg.Message<LancamentoExcluidoEvent>(m => m.SetEntityName("ex.flow-wise.lancamentos.events"));
});

builder.Services.AddNpgsql<LancamentosDbContext>(builder.Configuration.GetConnectionString("LancamentosDb"));
builder.Services.AddScoped<ILancamentoRepository, LancamentoRepository>();
builder.Services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
builder.Services.AddScoped<ILancamentoEventPublisher, MassTransitLancamentoEventPublisher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting FlowWise.Services.Lancamentos API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "FlowWise.Services.Lancamentos API terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}