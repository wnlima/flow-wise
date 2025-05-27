using FlowWise.Core;
using Serilog;
using FlowWise.Services.Consolidacao.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using FlowWise.Services.Consolidacao.Application.Queries;
using FlowWise.Services.Consolidacao.Domain.Interfaces;
using FlowWise.Services.Consolidacao.Application.EventConsumers;
using FlowWise.Services.Consolidacao.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços comuns do Flow Wise Core (infraestrutura compartilhada ou padronizada)
builder.Services.AddObservability(builder.Configuration, "Consolidacao_API");
builder.Services.AddFlowWiseCoreServices(builder.Configuration, typeof(GetSaldoDiarioQuery).Assembly);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<LancamentoRegistradoEventConsumer>();
    x.AddConsumer<LancamentoAtualizadoEventConsumer>();
    x.AddConsumer<LancamentoExcluidoEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"]!, builder.Configuration["RabbitMQ:VirtualHost"]!, h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"]!);
            h.Password(builder.Configuration["RabbitMQ:Password"]!);
        });

        cfg.ReceiveEndpoint("q.flow-wise.consolidacao.lancamentos_events", e =>
        {
            e.Bind("ex.flow-wise.lancamentos.events");
            e.ConfigureConsumer<LancamentoRegistradoEventConsumer>(context);
            e.ConfigureConsumer<LancamentoAtualizadoEventConsumer>(context);
            e.ConfigureConsumer<LancamentoExcluidoEventConsumer>(context);

            // [NFR-RES-003]: Implementar retry policies para falhas transitórias.
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5))); // Tenta 3 vezes, com intervalo de 5 segundos
            // [NFR-RES-003]: Implementar Circuit Breaker
            e.UseCircuitBreaker(cb =>
            {
                cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                cb.TripThreshold = 15; // 15% de falha
                cb.ActiveThreshold = 10; // 10 erros antes de abrir o circuito
                cb.ResetInterval = TimeSpan.FromMinutes(5); // Tempo para tentar fechar o circuito novamente
            });
            // O Rate Limit pode ser útil para cenários de alta concorrência se o processamento for lento.
            // Por hora, vamos omitir para o POC.
            // e.UseRateLimit(50, TimeSpan.FromSeconds(1)); 
        });
    });
});

builder.Services.AddNpgsql<ConsolidacaoDbContext>(builder.Configuration.GetConnectionString("ConsolidacaoDb"));
builder.Services.AddScoped<ISaldoDiarioRepository, SaldoDiarioRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting FlowWise.Services.Consolidacao API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "FlowWise.Services.Consolidacao API terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}