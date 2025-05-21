using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.Extensions.Configuration;
using MassTransit;
using Microsoft.Extensions.Logging;
using FlowWise.Common.Behaviors;
using System.Reflection;
using FluentValidation;

namespace FlowWise.Core
{
    /// <summary>
    /// Fornece métodos de extensão para configurar a injeção de dependência
    /// de serviços e componentes comuns (shared kernel) em toda a aplicação Flow Wise.
    /// Este projeto contém apenas configurações e abstrações estritamente compartilhadas,
    /// sem acoplar os domínios dos microsserviços.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adiciona os serviços de infraestrutura e padrões comuns (Serilog, Redis, MassTransit básico,
        /// Behaviors genéricos do MediatR e Autenticação JWT) à coleção de serviços.
        /// </summary>
        /// <param name="services">A coleção de serviços para estender.</param>
        /// <param name="configuration">A configuração da aplicação.</param>
        /// <returns>A coleção de serviços atualizada.</returns>
        public static IServiceCollection AddFlowWiseCoreServices(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblys)
        {
            // --- Configuração de Logging com Serilog ---
            // A configuração de sinks (Console, PostgreSQL) será feita no Program.cs de cada microsserviço
            // ou via appsettings.json, para dar flexibilidade.
            // Aqui, apenas a infraestrutura básica e enrichers comuns para injeção!
            services.AddSingleton<ILoggerFactory>(serviceProvider => new Serilog.Extensions.Logging.SerilogLoggerFactory(Log.Logger));
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            // --- Configuração do Cache Distribuído com Redis ---
            var redisConnectionString = configuration.GetConnectionString("RedisConnection") ?? "localhost:6379";
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "FlowWiseCache_";
            });

            // --- Configuração de Autenticação JWT ---
            //TODO: Adicionar o gerenciado de token

            // --- MediatR Pipeline Behaviors Genéricos (CorrelationId e Validação) ---
            // Esses behaviors são transversais e devem estar em FlowWise.Common.
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CorrelationIdBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // MediatR e FluentValidation para o assembly
            // Aqui, MediatR e FluentValidation são configurados para TRABALHAR COM AS CLASSES DESTE MICROSSERVIÇO.
            services.AddMediatR(cfg =>
            {
                foreach (var assembly in assemblys)
                {
                    cfg.RegisterServicesFromAssembly(assembly);
                }
            });

            foreach (var assembly in assemblys)
                services.AddValidatorsFromAssembly(assembly);

            return services;
        }

        /// <summary>
        /// Configura o MassTransit com RabbitMQ para a aplicação.
        /// Permite a configuração adicional do bus RabbitMQ através de um delegate.
        /// </summary>
        /// <param name="services">A coleção de serviços para estender.</param>
        /// <param name="configuration">A configuração da aplicação.</param>
        /// <param name="configureBus">Configuração adicional para o barramento</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureMassTransitWithRabbitMq(this IServiceCollection services, IConfiguration configuration, Action<IRabbitMqBusFactoryConfigurator> configureBus)
        {
            services.AddMassTransit(x =>
            {
                x.AddPublishMessageScheduler();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:Host"]!, configuration["RabbitMQ:VirtualHost"]!, h =>
                    {
                        h.Username(configuration["RabbitMQ:Username"]!);
                        h.Password(configuration["RabbitMQ:Password"]!);
                    });

                    configureBus?.Invoke(cfg);
                });
            });

            return services;
        }
    }
}